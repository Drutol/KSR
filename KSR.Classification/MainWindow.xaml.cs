using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

namespace KSR.Classification
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var res = new DocumentReader().ObtainVectorSpaceModels().Take(100).ToList();
            var tasks = new List<Task<WeightMatrix>>();
            for (int i = 0; i < 10; i++)
            {
                var i1 = i;
                tasks.Add(Task.Run(() => new WeightMatrix(res,i1)));
            }
            await Task.WhenAll(tasks);
            var finalMatrix = new WeightMatrix(tasks.Select(task => task.Result));
            for (int i = 20; i < 30; i++)
            {
                var dist = finalMatrix.GetDistance(res[i]);
                Debug.WriteLine($"Article tags: {string.Join(",",res[i].Tags)}");
                Debug.WriteLine($"Top results:");
                foreach (var valueTuple in dist.OrderBy(tuple => tuple.Distance).Take(3))
                {
                    Debug.WriteLine($"Matching tags: {string.Join(",", valueTuple.Article.Tags)}");
                }
                Debug.WriteLine($"\n\n");
            }
        }
    }
}
