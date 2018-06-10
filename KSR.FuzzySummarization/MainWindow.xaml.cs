using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using KSR.FuzzySummarization.DataProcessing;
using KSR.FuzzySummarization.FuzzyLogic;
using KSR.FuzzySummarization.Model;
using Newtonsoft.Json;

namespace KSR.FuzzySummarization
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var data = new DataExtractor().ObtainRecords().ToList();
            var variables =
                JsonConvert.DeserializeObject<List<LinguisticVariable>>(
                    File.ReadAllText("Data/linguisticVariables.json"));
            var quantifiers =
                JsonConvert.DeserializeObject<List<LinguisticVariable>>(
                    File.ReadAllText("Data/linguisticQuantifiers.json"));
            var qualificators =
                JsonConvert.DeserializeObject<List<LinguisticVariable>>(
                    File.ReadAllText("Data/linguisticQualificators.json"));

            var groupings = variables.GroupBy(variable => variable.MemberToExtract).ToList();
            var sets = new List<FuzzySet>();
            foreach (var grouping in groupings)
            {
                var movedGroping = new List<Tuple<string, List<LinguisticVariable>>>
                {
                    new Tuple<string, List<LinguisticVariable>>(grouping.Key, grouping.ToList())
                };
                movedGroping.AddRange(groupings.Where(linguisticVariables => linguisticVariables != grouping).Take(2)
                    .Select(g => new Tuple<string, List<LinguisticVariable>>(g.Key, g.ToList())));
                sets = sets.Concat(GetSets(data, movedGroping)).ToList();
            }

            var i = 0;
            var sw = new Stopwatch();
            sw.Start();
            var summarizations = new List<SummarizationResult>();
            var qualities = new List<double>();
            foreach (var fuzzySet in sets)
            {
                Process(fuzzySet);
                fuzzySet.Qualificator = new FuzzySet(data, qualificators[i++]);
                if (i == qualificators.Count)
                    i = 0;
                Process(fuzzySet);


                void Process(FuzzySet set)
                {
                    var result = new SummarizationResult();
                    var best = "";
                    double quality = 0;
                    foreach (var quantifier in quantifiers)
                    {
                        var t = set.DegreeOfTruth(quantifier);
                        var summarization = $"{quantifier.Name} people {set} [Quality: {t:N5}]";
                        if (t > quality)
                        {
                            best = summarization;
                            quality = t;
                        }

                        result.AllSummarizations.Add(summarization);
                    }

                    qualities.Add(quality);
                    result.BestSummarization = best;
                    summarizations.Add(result);
                }
            }

            sw.Stop();
            var avg = qualities.Average();
            Results.ItemsSource = summarizations;
        }

        private List<FuzzySet> GetSets(IEnumerable<DataRecord> data,
            IEnumerable<Tuple<string, List<LinguisticVariable>>> vars)
        {
            var sets = new List<FuzzySet>();
            var i = 1;
            vars = vars.Take(1);
            foreach (var group in vars)
            {
                if (!sets.Any())
                {
                    foreach (var linguisticVariable in group.Item2) sets.Add(new FuzzySet(data, linguisticVariable));
                }
                else
                {
                    var combo = new List<FuzzySet>();
                    foreach (var fuzzySet in sets)
                    {
                        var j = 0;
                        foreach (var linguisticVariable in group.Item2)
                        {
                            if (j < sets.Count - 1 || fuzzySet.HasOr)
                                combo.Add(fuzzySet & new FuzzySet(data, linguisticVariable));
                            else
                                combo.Add(fuzzySet | new FuzzySet(data, linguisticVariable));
                            j++;
                        }
                    }

                    sets = combo;
                }

                i++;
            }

            return sets;
        }
    }
}