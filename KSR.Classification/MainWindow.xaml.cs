using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using KSR.Classification.Models;

namespace KSR.Classification
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly int[] _progress = new int[20];
        private int _totalParts;
        private List<Article> _readDocuments;

        private List<string> _validPlaces = new List<string>
        {
            "west-germany", "usa", "france", "uk", "canada", "japan"
        };

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            _totalParts = 0;
            var cts = new CancellationTokenSource();
            StartObservingProgress(cts.Token);


            var tasks = new List<Task<WeightMatrix>>();
            var topic = CategoriesComboBox.SelectedItem as string;

            var docSet = _readDocuments
                .Where(article => article.Tags.ContainsKey(topic)).ToList();
            if (topic == "places")
                docSet = docSet.Where(article => article.Tags.ContainsKey("places") &&
                        article.Tags["places"].Count == 1 && _validPlaces.Any(s => s.Equals(article.Tags["places"][0])))
                    .ToList();
            

            var wordFrequencies = new Dictionary<string,double>();
            var allWords = docSet.SelectMany(article => article.Words).ToList();
            var distinctWords = allWords.Distinct().ToList();

            var freqTasks = new List<Task<Dictionary<string,double>>>();
            for (int i = 0; i < 10; i++)
            {
                var i1 = i;
                freqTasks.Add(Task.Factory.StartNew(() =>
                {
                    var output = new Dictionary<string,double>();
                    var words = distinctWords.Skip(i1 * distinctWords.Count / 10).Take(distinctWords.Count / 10).ToList();
                    int count = 0;
                    foreach (var word in words)
                    {
                        Debug.WriteLine($"{count++}/{words.Count}");
                        output[word] = allWords.Count(
                                           s => s.Equals(word, StringComparison.CurrentCultureIgnoreCase)) /
                                       (double)allWords.Count;
                    }
                    return output;
                }));
            }

            LoadingDocumentsGrid.Visibility = Visibility.Visible;
            await Task.WhenAll(freqTasks);
            LoadingDocumentsGrid.Visibility = Visibility.Collapsed;

            var freqResults = freqTasks.Select(task => task.Result);
            foreach (var freqResult in freqResults)
            {
                foreach (var d in freqResult)
                {
                    wordFrequencies[d.Key] = d.Value;
                }
            }

            var finalWords = wordFrequencies.OrderByDescending(tuple => tuple.Value).Take(1000).Select(tuple => tuple.Key).ToList();


            for (int i = 0; i < 20; i++)
            {
                var i1 = i;
                tasks.Add(Task.Factory.StartNew(() => new WeightMatrix(docSet, finalWords.ToList(), i1,
                    counter => _progress[i1] = counter,
                    i1 == 0 ? new Action<int>(i2 => _totalParts = i2 * 20) : null)));
            }

            await Task.WhenAll(tasks);
            cts.Cancel();
            ProgressBar.Value = _totalParts;
            ProgressLabel.Text = $"{_totalParts}/{_totalParts}";
            var finalMatrix = new WeightMatrix(tasks.Select(task => task.Result));
            var results = new List<ClassifiedArticle>();
            var articlesToTake = (int)(DataDivisionSlider.Value * docSet.Count / 100);
            LoadingDocumentsGrid.Visibility = Visibility.Visible;
            var k = (int) KSlider.Value;
            await Task.Run(() =>
            {
                for (int i = 0; i < articlesToTake; i++)
                {

                    var dist = finalMatrix.GetDistance(docSet[i]);
                    var neighbours = dist.OrderBy(tuple => tuple.Distance).Skip(1).Take(k)
                        .Select(tuple => tuple.Article).ToList();
                    var buckets = new Dictionary<string, int>();
                    var bucketKeys = new Dictionary<string, Article>();
                    foreach (var neighbour in neighbours)
                    {
                        var key = string.Join("", neighbour.Tags[topic].OrderBy(t => t[0]));
                        if (!bucketKeys.ContainsKey(key))
                            bucketKeys.Add(key, neighbour);
                        if (!buckets.ContainsKey(key))
                            buckets.Add(key, 0);
                        buckets[key]++;
                    }

                    results.Add(new ClassifiedArticle(topic)
                    {
                        Article = docSet[i],
                        Neighbours =
                            new List<Article> {bucketKeys[buckets.OrderByDescending(pair => pair.Value).First().Key]}
                    });
                }
            });
            LoadingDocumentsGrid.Visibility = Visibility.Collapsed;
            var correct = results.Count(article => article.IsMatch);
            AccuracyBox.Text = $"{100 * correct / articlesToTake}% ({correct}/{articlesToTake})";

            ResultsView.ItemsSource = results;
        }

        private async void StartObservingProgress(CancellationToken  token)
        {
            bool maxDetermined = false;
            ProgressBar.IsIndeterminate = true;
            ProgressLabel.Text = "Preparing...";
            try
            {
                while (true)
                {
                    if (token.IsCancellationRequested)
                        return;

                    if (_totalParts != 0 && !maxDetermined)
                    {
                        maxDetermined = true;

                        ProgressBar.IsIndeterminate = false;
                        ProgressBar.Maximum = _totalParts;
                    }

                    ProgressBar.Value = _progress.Sum();
                    if(_totalParts != 0)
                        ProgressLabel.Text = $"{ProgressBar.Value}/{_totalParts}";


                    await Task.Delay(500, token);
                }
            }
            catch (OperationCanceledException)
            {
                
            }
        }

        private async void AnalyseDocuments(object sender, RoutedEventArgs e)
        {
            LoadingDocumentsGrid.Visibility = Visibility.Visible;
            await Task.Run(() => { _readDocuments = new DocumentReader().ObtainVectorSpaceModels().ToList(); });
            LoadingDocumentsGrid.Visibility = Visibility.Collapsed;

            ArticlesCountLabel.Text = _readDocuments.Count.ToString();
            CategoriesComboBox.ItemsSource =
                _readDocuments.SelectMany(article => article.Tags).Select(pair => pair.Key).Distinct().ToList();
            CategoriesComboBox.SelectedIndex = 0;
        }

        private void CategoriesComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var topic = CategoriesComboBox.SelectedItem as string;
            var docSet = new List<Article>();
            if (topic == "places")
                docSet = _readDocuments.Where(article => article.Tags.ContainsKey("places") &&
                        article.Tags["places"].Count == 1 && _validPlaces.Any(s => s.Equals(article.Tags["places"][0])))
                    .ToList();
            else
                docSet = _readDocuments;
            ArticlesWithCountLabel.Text = docSet
                .Count(article => article.Tags.ContainsKey(topic)).ToString();
        }
    }
}
