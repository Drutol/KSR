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
            foreach (var linguisticVariable in variables)
            {
                var set = new FuzzySet(data, linguisticVariable);
                var support = set.Support.ToList();
                Debug.WriteLine($"{support.Count} people are {linguisticVariable.Name}");
            }
            
            
        }
    }
}
