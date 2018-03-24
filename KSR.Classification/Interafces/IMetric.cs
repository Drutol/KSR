using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSR.Classification.Interafces
{
    public interface IMetric
    {
        double GetDistance(List<double> first, List<double> second);
    }
}
