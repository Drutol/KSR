using System;
using System.Collections;
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

        private readonly List<LinguisticVariable> _allUnderlyingVariables = new List<LinguisticVariable>();
        public List<LinguisticVariable> Flatten()
        {
            if (_allUnderlyingVariables.Any())
                return _allUnderlyingVariables;

            _allUnderlyingVariables.Add(Variable);
            var child = Child;
            while (child != null)
            {
                _allUnderlyingVariables.Add(child.Variable);
                child = child.Child;
            }

            return _allUnderlyingVariables;
        }
    }
}
