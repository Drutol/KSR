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
        private readonly int[] _progress = new int[10];
        private int _totalParts;
        private List<Article> _readDocuments;

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

            var docSet = _readDocuments
                .Where(article => article.Tags.ContainsKey(CategoriesComboBox.SelectedItem as string)).ToList();

            for (int i = 0; i < 10; i++)
            {
                var i1 = i;
                tasks.Add(Task.Run(() => new WeightMatrix(docSet, i1, counter => _progress[i1] = counter,
                    i1 == 0 ? new Action<int>(i2 => _totalParts = i2*10) : null)));
            }
            await Task.WhenAll(tasks);
            cts.Cancel();

            var finalMatrix = new WeightMatrix(tasks.Select(task => task.Result));
            var rand = new Random();
            for (int i = 0; i < 30; i++)
            {
                var index = rand.Next(docSet.Count);
                try
                {                   
                    var dist = finalMatrix.GetDistance(docSet[index]);

                    Debug.WriteLine($"Article tags: {string.Join(",", docSet[index].Tags)}");
                    Debug.WriteLine($"Top results:");
                    foreach (var valueTuple in dist.OrderBy(tuple => tuple.Distance).Take(3))
                    {
                        Debug.WriteLine($"Matching tags: {string.Join(",", valueTuple.Article.Tags)}");
                    }
                    Debug.WriteLine($"\n\n");
                }
                catch (Exception exception)
                {

                }
            }
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
            ArticlesWithCountLabel.Text = _readDocuments
                .Count(article => article.Tags.ContainsKey(CategoriesComboBox.SelectedItem as string)).ToString();
        }
    }
}
