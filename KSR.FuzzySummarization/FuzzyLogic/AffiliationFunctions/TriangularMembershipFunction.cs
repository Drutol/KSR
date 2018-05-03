using System;
using System.Collections.Generic;
using KSR.FuzzySummarization.Interfaces;

namespace KSR.FuzzySummarization.FuzzyLogic.AffiliationFunctions
{
    public class TriangularMembershipFunction : IMembershipFunction
    {
        private List<double> _boundaries;
        private double _a, _b, _c;

        public double GetMembership(double x)
        {
            if (Math.Abs(x - _b) < .00001)
            {
                return 1;
            }

            if (x > _a && x < _b)
            {
                return 1.0 / (_b - _a) * x + 1.0 - (1.0 / (_b - _a)) * _b;
            }

            if (x > _b && x < _c)
            {
                return 1.0 / (_b - _c) * x + 1.0 - (1.0 / (_b - _c)) * _b;
            }

            return 0;
        }

        public List<double> Parameters
        {
            get => _boundaries;
            set
            {
                if(value.Count != 3)
                    throw new ArgumentException();
                _boundaries = value;
                _a = value[0];
                _b = value[1];
                _c = value[2];
            }
        }
    }
}
