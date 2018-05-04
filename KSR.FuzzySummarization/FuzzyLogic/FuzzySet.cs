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
        private readonly LinguisticVariable _linguisticVariable;
        private readonly Dictionary<DataRecord, double> _memberships = new Dictionary<DataRecord, double>();

        public  FuzzySet(IEnumerable<DataRecord> elements, LinguisticVariable linguisticVariable)
        {
            _elements = elements.ToList();
            _linguisticVariable = linguisticVariable;
        }

        public double this[DataRecord item]
        {
            get
            {
                if (_memberships.ContainsKey(item))
                    return _memberships[item];
                var membership = _linguisticVariable.MembershipFunction.GetMembership(_linguisticVariable.IsQuantifier
                    ? _linguisticVariable.MembershipFunction.GetMembership(_elements.Count)
                    : _linguisticVariable.Extractor(item));
                _memberships[item] = membership;
                return membership;
            }
        }

        public IEnumerable<DataRecord> Support => _elements.Where(record => this[record] > 0);
    }
}
