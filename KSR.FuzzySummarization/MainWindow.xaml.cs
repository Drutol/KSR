using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
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
using Aq.ExpressionJsonSerializer;
using KSR.FuzzySummarization.DataProcessing;
using KSR.FuzzySummarization.FuzzyLogic;
using KSR.FuzzySummarization.FuzzyLogic.AffiliationFunctions;
using KSR.FuzzySummarization.Model;
using Newtonsoft.Json;

namespace KSR.FuzzySummarization
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
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

            int i = 0;
            var summarizations = new List<SummarizationResult>();
            foreach (var set in sets)
            {
                set.Qualificator = new FuzzySet(data, qualificators[i++]);
                if (i == qualificators.Count)
                    i = 0;
                var result = new SummarizationResult();
                string best = "";
                double quality = 0;
                foreach (var quantifier in quantifiers)
                {
                    var t = set.DegreeOfTruth(quantifier);
                    var summarization = $"{quantifier.Name} people {set} [{t}]";
                    if (t > quality)
                    {
                        best = summarization;
                        quality = t;
                    }
                    
                    result.AllSummarizations.Add(summarization);
                }

                result.BestSummarization = best;
                summarizations.Add(result);
            }

            Results.ItemsSource = summarizations;
        }

        private List<FuzzySet> GetSets(IEnumerable<DataRecord> data, IEnumerable<Tuple<string,List<LinguisticVariable>>> vars)
        {
            List<FuzzySet> sets = new List<FuzzySet>();
            int i = 1;
            var totalGroups = vars.Count();
            foreach (var group in vars)
            {
                if (!sets.Any())
                {
                    foreach (var linguisticVariable in group.Item2)
                    {
                        sets.Add(new FuzzySet(data, linguisticVariable));
                    }
                }
                else
                {
                    var combo = new List<FuzzySet>();
                    foreach (var fuzzySet in sets)
                    {
                        foreach (var linguisticVariable in group.Item2)
                        {
                            if (i == totalGroups || fuzzySet.HasOr)
                                combo.Add(fuzzySet & new FuzzySet(data, linguisticVariable));
                            else
                                combo.Add(fuzzySet | new FuzzySet(data, linguisticVariable));
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
