using System.Collections.Generic;

namespace KSR.FuzzySummarization.Model
{
    public class SummarizationResult
    {
        public string BestSummarization { get; set; }
        public List<string> AllSummarizations { get; set; } = new List<string>();
    }
}