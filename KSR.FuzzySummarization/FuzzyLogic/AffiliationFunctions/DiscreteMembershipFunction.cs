using System;
using System.Collections.Generic;
using KSR.FuzzySummarization.Interfaces;

namespace KSR.FuzzySummarization.FuzzyLogic.AffiliationFunctions
{
    public class DiscreteMembershipFunction : IMembershipFunction
    {
        private readonly Dictionary<int, double> _memberships = new Dictionary<int, double>();
        private List<double> _boundaries;

        public double GetMembership(double x)
        {
            return _memberships[(int) x];
        }

        public List<double> Parameters
        {
            get => _boundaries;
            set
            {
                if (value.Count < 2 || value.Count % 2 != 0)
                    throw new ArgumentException();
                _boundaries = value;
                for (var i = 0; i < value.Count; i += 2) _memberships[(int) value[i]] = value[i + 1];
            }
        }
    }
}