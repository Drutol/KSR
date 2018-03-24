using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSR.Classification.Models
{
    public class ClassifiedArticle
    {
        private readonly string _classificationTopic;

        public ClassifiedArticle(string classificationTopic)
        {
            _classificationTopic = classificationTopic;
        }

        public Article Article { get; set; }
        public List<Article> Neighbours { get; set; }

        public string MatchedTags => string.Join(",", Neighbours.SelectMany(article => article.Tags[_classificationTopic]).Distinct());

        public string OriginalTags => string.Join(",", Article.Tags[_classificationTopic].Distinct());

        public bool IsMatch => Neighbours.SelectMany(article => article.Tags[_classificationTopic]).Any(tag =>
            Article.Tags[_classificationTopic].Any(correctTag => correctTag.Equals(tag)));
    }
}
