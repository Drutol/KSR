﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KSR.FuzzySummarization.Interfaces;

namespace KSR.FuzzySummarization.FuzzyLogic.AffiliationFunctions
{
    public class DiscreteMembershipFunction : IMembershipFunction
    {
        private List<double> _boundaries;
        private readonly Dictionary<int, double> _memberships = new Dictionary<int, double>();

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
                for (int i = 0; i < value.Count; i+=2)
                {
                    _memberships[(int) value[i]] = value[i + 1];
                }
            }
        }
    }
}
