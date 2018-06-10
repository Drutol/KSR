using System.Collections.Generic;

namespace KSR.FuzzySummarization.Interfaces
{
    public interface IMembershipFunction
    {
        List<double> Parameters { get; set; }
        double GetMembership(double x);
    }
}