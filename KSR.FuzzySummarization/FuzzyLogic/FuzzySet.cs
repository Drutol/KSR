using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using KSR.FuzzySummarization.Model;

namespace KSR.FuzzySummarization.FuzzyLogic
{
    public class FuzzySet
    {
        private readonly LinguisticVariableGroup _linguisticVariableGroup;
        private readonly Dictionary<DataRecord, double> _memberships = new Dictionary<DataRecord, double>();
        private List<DataRecord> _allElements;
        private List<DataRecord> _elements;
        private FuzzySet _qualificator;


        public FuzzySet(IEnumerable<DataRecord> elements, LinguisticVariable linguisticVariable)
        {
            _elements = elements.ToList();
            _linguisticVariableGroup =
                new LinguisticVariableGroup
                {
                    Variable = linguisticVariable
                };
        }

        private FuzzySet(IEnumerable<DataRecord> elements, LinguisticVariableGroup linguisticVariableGroup)
        {
            _elements = elements.ToList();
            _linguisticVariableGroup = linguisticVariableGroup;
        }

        public FuzzySet Qualificator
        {
            get => _qualificator;
            set
            {
                _qualificator = value;
                _allElements = _elements.ToList();
                _elements = _qualificator.Support.ToList();
            }
        }

        public double this[DataRecord item]
        {
            get
            {
                if (_memberships.ContainsKey(item))
                    return _memberships[item];

                var membership = GetMembership(_linguisticVariableGroup);

                _memberships[item] = membership;

                return membership;

                double GetMembership(LinguisticVariableGroup group)
                {
                    if (group.Child != null)
                    {
                        var mem = GetMembership(group.Child);
                        return group.RelationToChild(new List<double> {mem, GetOwnMembership()});
                    }

                    return GetOwnMembership();

                    double GetOwnMembership()
                    {
                        return group.Variable.MembershipFunction.GetMembership(group.Variable.IsQuantifier
                            ? group.Variable.MembershipFunction.GetMembership(_elements.Count)
                            : group.Variable.Extractor(item));
                    }
                }
            }
        }

        public IEnumerable<DataRecord> Support => _elements.Where(record => this[record] > 0);

        public double CardinalNumber => _memberships.Sum(pair => pair.Value);

        public bool HasOr
        {
            get
            {
                var current = _linguisticVariableGroup;
                while (current != null)
                {
                    if (current.RelationName == "or") return true;

                    current = current.Child;
                }

                return false;
            }
        }

        public static FuzzySet operator &(FuzzySet first, FuzzySet other)
        {
            var group = new LinguisticVariableGroup
            {
                Variable = other._linguisticVariableGroup.Variable,
                Child = first._linguisticVariableGroup,
                RelationToChild = list => list.Min(),
                RelationName = "and"
            };
            return new FuzzySet(first._elements, group);
        }

        public static FuzzySet operator |(FuzzySet first, FuzzySet other)
        {
            var group = new LinguisticVariableGroup
            {
                Variable = other._linguisticVariableGroup.Variable,
                Child = first._linguisticVariableGroup,
                RelationToChild = list => list.Max(),
                RelationName = "or"
            };
            return new FuzzySet(first._elements, group);
        }

        public override string ToString()
        {
            var output = "";
            var currentGroup = _linguisticVariableGroup;
            if (Qualificator != null)
                output += $"being {Qualificator._linguisticVariableGroup.Variable.Name} are ";
            else
                output = "are ";

            while (currentGroup != null)
            {
                output += $"{currentGroup.Variable.Name} ";
                if (currentGroup.Child != null)
                {
                    output += $"{currentGroup.RelationName} are ";
                    currentGroup = currentGroup.Child;
                }
                else
                {
                    break;
                }
            }

            return output;
        }

        public double DegreeOfTruth(LinguisticVariable quantifier)
        {
            var degrees = new List<double>();

            //t1
            degrees.Add(CardinalNumber / _elements.Count);
            //t2
            var t2 = 1d;
            var allVariables = _linguisticVariableGroup.Flatten();
            foreach (var variable in allVariables)
            {
                var tempSet = new FuzzySet(_elements, variable);
                var factor = tempSet.Support.Count() / (double) _elements.Count;
                t2 *= factor;
            }

            degrees.Add(1 - Math.Pow(t2, t2 / allVariables.Count));
            //t3
            var t3 = Support.Count() / (double) _elements.Count;
            degrees.Add(t3);
            //t4
            var t4 = 1d;
            foreach (var variable in allVariables)
            {
                var tempSet = new FuzzySet(_elements, variable);
                t4 *= tempSet.Support.Count() / (double) _elements.Count - t3;
            }

            degrees.Add(Math.Abs(t4));
            //t5
            degrees.Add(2 * Math.Pow(0.5, allVariables.Count));

            //t6
            var quantifierElements = quantifier.MembershipFunctionParameters.Last() -
                                     quantifier.MembershipFunctionParameters.First();
            degrees.Add(1 - quantifierElements / _elements.Count);

            //t7
            var quantifierSet = Enumerable.Range((int) quantifier.MembershipFunctionParameters.First(),
                (int) quantifier.MembershipFunctionParameters.Last() - 1);
            var quantifierCardinalNumber = quantifierSet.Sum(i => quantifier.MembershipFunction.GetMembership(i));
            degrees.Add(Math.Min(1,quantifierCardinalNumber / quantifierElements));

            //t8
            var sc = Support.Count();
            var f = 1 + (_elements.Count / (double)(_allElements ?? _elements).Count);
            degrees.Add(quantifier.MembershipFunction.GetMembership(sc * f));

            if (Qualificator != null)
            {   
                //t9
                degrees.Add(1 - Qualificator.Support.Count() / (double)_allElements.Count);
                //t10
                degrees.Add(1 - Qualificator.CardinalNumber / _allElements.Count);
                //t11
                degrees.Add(2 * Math.Pow(0.5, Qualificator._linguisticVariableGroup.Flatten().Count));
            }
            
            //if(degrees.Any(d => d < 0 || d > 1))
            //    Debugger.Break();

            return degrees.Average();
        }
    }
}