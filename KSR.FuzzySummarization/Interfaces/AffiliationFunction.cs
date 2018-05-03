using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSR.FuzzySummarization.Interfaces
{
    public interface IMembershipFunction
    {
        double GetMembership(double x);
        List<double> Parameters { get; set; }
    }
}
