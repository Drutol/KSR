using System;
using System.Collections.Generic;
using System.Linq;

namespace KSR.FuzzySummarization.FuzzyLogic
{
    public class LinguisticVariableGroup
    {
        private readonly List<LinguisticVariable> _allUnderlyingVariables = new List<LinguisticVariable>();
        public LinguisticVariable Variable { get; set; }
        public LinguisticVariableGroup Child { get; set; }
        public Func<List<double>, double> RelationToChild { get; set; }
        public string RelationName { get; set; }

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