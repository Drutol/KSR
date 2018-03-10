using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSR.Classification.Models
{
    public class Article
    {
        public string Title { get; set; }
        public Dictionary<string,List<string>> Tags { get; set; }
        public List<string> Words { get; set; }
    }
}
