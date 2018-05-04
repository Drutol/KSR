using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSR.FuzzySummarization.FuzzyLogic
{
    public class LinguisticVariableGroup
    {
        public LinguisticVariable Variable { get; set; }
        public LinguisticVariableGroup Child { get; set; }
        public Func<List<double>,double> RelationToChild { get; set; }
        public string RelationName { get; set; }
    }
}
