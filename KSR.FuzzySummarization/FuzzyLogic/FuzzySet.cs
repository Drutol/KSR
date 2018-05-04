using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KSR.FuzzySummarization.Interfaces;
using KSR.FuzzySummarization.Model;

namespace KSR.FuzzySummarization.FuzzyLogic
{
    public class FuzzySet
    {
        private readonly List<DataRecord> _elements;
        private readonly LinguisticVariableGroup _linguisticVariableGroup;
        //private readonly Dictionary<DataRecord, double> _memberships = new Dictionary<DataRecord, double>();

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

        public double this[DataRecord item]
        {
            get
            {
                //if (_memberships.ContainsKey(item))
                //    return _memberships[item];
                //var membership = GetMembership(_linguisticVariable);
                //_memberships[item] = membership;
                //return membership;
                return GetMembership(_linguisticVariableGroup);

                double GetMembership(LinguisticVariableGroup group)
                {
                    if (group.Child != null)
                    {
                        var mem = GetMembership(group.Child);
                        return group.RelationToChild(new List<double> {mem,GetOwnMembership()});
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
            string output = "";
            var currentGroup = _linguisticVariableGroup;
            while (currentGroup != null)
            {
                output += $"{currentGroup.Variable.Name} ";
                if (currentGroup.Child != null)
                {
                    output += $"{currentGroup.RelationName} are ";
                    currentGroup = currentGroup.Child;
                }
                else
                    break;
            }

            return output;
        }

        public IEnumerable<DataRecord> Support => _elements.Where(record => this[record] > 0);

        public bool HasOr
        {
            get
            {
                var current = _linguisticVariableGroup;
                while (current != null)
                {
                    if (current.RelationName == "or")
                    {
                        return true;
                    }

                    current = current.Child;
                }

                return false;
            }
        }
    }
}
