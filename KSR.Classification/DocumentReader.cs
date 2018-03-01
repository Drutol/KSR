using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using HtmlAgilityPack;
using KSR.Classification.Models;

namespace KSR.Classification
{
    public class DocumentReader
    {
        public IEnumerable<Article> ObtainVectorSpaceModels()
        {
            for (int i = 0; i < 1; i++)
            {
                var rawXml = File.ReadAllText($"Data/reut2-{i.ToString().PadLeft(3, '0')}.sgm");
                var doc = new HtmlDocument();
                doc.LoadHtml(rawXml);
                var regex = new Regex("[^a-zA-Z0-9 -]");

                foreach (var article in doc.DocumentNode.Descendants("REUTERS"))
                {
                    var body = article.Descendants("BODY").FirstOrDefault();
                    var places = article.Descendants("PLACES").FirstOrDefault()?.Descendants("D");
                    if (body != null && places != null)
                    {
                        yield return new Article
                        {                         
                            Words = regex.Replace(body.InnerText, " ").ToLower().Split(' ').Where(s => s.Length > 2).ToList(),
                            Tags = places.Select(node => node.InnerText).ToList()
                        };
                    }
                }
            }
        }
    }
}
