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
                JsonConvert.DeserializeObject<List<LinguisticVariable>>(File.ReadAllText("Data/linguisticVariables.json"));
            var quantifiers =
                JsonConvert.DeserializeObject<List<LinguisticVariable>>(File.ReadAllText("Data/linguisticQuantifiers.json"));
            foreach (var linguisticVariableGroup in variables.GroupBy(variable => variable.MemberToExtract))
            {
                foreach (var linguisticVariable in linguisticVariableGroup)
                {
                    var set = new FuzzySet(data, linguisticVariable);
                    (LinguisticVariable Quantifier, double Match) mostMatchingQuantifier = (null, 0);
                    foreach (var quantifier in quantifiers)
                    {
                        var match = quantifier.MembershipFunction.GetMembership(set.Support.Count());
                        if (match > mostMatchingQuantifier.Match)
                            mostMatchingQuantifier = (quantifier, match);
                    }
                    Debug.WriteLine($"{mostMatchingQuantifier.Quantifier.Name} people are {linguisticVariable.Name}");
                }
            }                   
        }
    }
}
