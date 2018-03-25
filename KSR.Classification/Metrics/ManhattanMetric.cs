using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KSR.Classification.Interafces;

namespace KSR.Classification.Metrics
{
    public class ManhattanMetric : IMetric
    {
        public double GetDistance(List<double> first, List<double> second)
        {
            double distance = 0;

            for (int i = 0; i < first.Count; i++)
            {
                distance += Math.Abs(first[i] - second[i]);
            }

            return distance;
        }
    }
}
