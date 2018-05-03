using System;
using System.Collections.Generic;
using KSR.FuzzySummarization.Interfaces;

namespace KSR.FuzzySummarization.FuzzyLogic.AffiliationFunctions
{
    public class TrapezoidalMembershipFunction : IMembershipFunction
    {
        private List<double> _boundaries;
        private double _a, _b, _c, _d;

        public double GetMembership(double x)
        {
            if (x >= _b && x <= _c)
            {
                return 1;
            }

            if (x > _a && x < _b)
            {
                return 1.0 / (_b - _a) * x + 1.0 - (1.0 / (_b - _a)) * _b;
            }

            if (x > _c && x < _d)
            {
                return 1.0 / (_c - _d) * x + 1.0 - (1.0 / (_c - _d)) * _c;
            }

            return 0;
        }

        public List<double> Parameters
        {
            get => _boundaries;
            set
            {
                if (value.Count != 4)
                    throw new ArgumentException();
                _boundaries = value;
                _a = value[0];
                _b = value[1];
                _c = value[2];
                _d = value[3];
            }
        }
    }
}
