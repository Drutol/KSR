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


            var data = new DataExtractor().ObtainRecords().ToList();
            var variables =
                JsonConvert.DeserializeObject<List<LinguisticVariable>>(
                    File.ReadAllText("Data/linguisticVariables.json"));
            var quantifiers =
                JsonConvert.DeserializeObject<List<LinguisticVariable>>(
                    File.ReadAllText("Data/linguisticQuantifiers.json"));

            var groupings = variables.GroupBy(variable => variable.MemberToExtract).ToList();
            var sets = new List<FuzzySet>();
            foreach (var grouping in groupings)
            {
                var movedGroping = new List<Tuple<string, List<LinguisticVariable>>>();
                movedGroping.Add(new Tuple<string, List<LinguisticVariable>>(grouping.Key,grouping.ToList()));
                foreach (var g in groupings.Where(linguisticVariables => linguisticVariables != grouping).Take(2))
                {
                    movedGroping.Add(new Tuple<string, List<LinguisticVariable>>(g.Key,g.ToList()));
                }
                sets = sets.Concat(GetSets(data, movedGroping)).ToList();
            }
            foreach (var set in sets)
            {
                (LinguisticVariable Quantifier, double Match, double ExactCount) mostMatchingQuantifier = (null, 0, 0);
                foreach (var quantifier in quantifiers)
                {
                    var supportCount = set.Support.Count();
                    var match = quantifier.MembershipFunction.GetMembership(supportCount);
                    if (match > mostMatchingQuantifier.Match)
                        mostMatchingQuantifier = (quantifier, match, supportCount);
                }

                Debug.WriteLine(
                    $"[{mostMatchingQuantifier.ExactCount}]{mostMatchingQuantifier.Quantifier.Name} people are {set}");
            }
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
