using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KSR.Classification.Interafces;
using KSR.Classification.Metrics;
using KSR.Classification.Models;

namespace KSR.Classification
{
    public class WeightMatrix
    {
        public static IMetric CurrentMetric { get; set; }

        private readonly int _correlationId;
        public Dictionary<string, List<double>> _weights = new Dictionary<string, List<double>>();
        private List<string> _distinctWords;
        private List<Article> _articles;

        public WeightMatrix(IEnumerable<WeightMatrix> matrices)
        {
            var first = matrices.First();

            _articles = first._articles;
            _distinctWords = first._distinctWords;

            foreach (var weightMatrix in matrices.OrderByDescending(matrix => matrix._correlationId))
            {
                foreach (var weightMatrixWeight in weightMatrix._weights)
                {
                    _weights[weightMatrixWeight.Key] = weightMatrixWeight.Value;
                }
            }
        }

        public WeightMatrix(List<Article> articles, List<string> allWords, int correlationId, Action<int> onProgress,
            Action<int> onPartEvaluated = null)
        {
            _correlationId = correlationId;
            _articles = articles.ToList();
            _distinctWords = allWords;

            var part = _distinctWords.Count / 20;
            int counter = 0;
            var part2 = part;
            if (correlationId == 19)
                part2 += 20;
            onPartEvaluated?.Invoke(part2);

            for (int i = part * correlationId; i < part * correlationId + part2 && i < _distinctWords.Count; i++)
            {
                i = i < 0 ? 0 : i;
                var weightsForWord = new List<double>();
                var termFrequencyInDocuments =
                    allWords.Count(s1 => s1.Equals(_distinctWords[i])) / (double) allWords.Count;
                Debug.WriteLine($"{counter++}/{part}");
                onProgress(counter);
                foreach (var article in _articles)
                {
                    double weightSum = 0;
                    var freq = GetFrequency(article, _distinctWords[i]);
                    //var inverseDocFreq = freq * Math.Log(_articles.Count, 2);
                    //if (freq != 0)
                    //{
                    //    foreach (var t in _distinctWords)
                    //    {
                    //        var temp = article.Words.Count(s1 => s1.Equals(t));
                    //        var temp2 = temp * Math.Log(_articles.Count / termFrequencyInDocuments, 2);
                    //        weightSum += temp2 * temp2;
                    //    }
                    //    weightsForWord.Add(inverseDocFreq / weightSum);
                    //}
                    //else
                    //{
                    //    weightsForWord.Add(0);
                    //}
                    
                    weightsForWord.Add(freq);
                }

                _weights.Add(_distinctWords[i], weightsForWord);
            }
        }

        public List<(Article Article, double Distance)> GetDistance(Article article)
        {
            var articleIndex = _articles.IndexOf(article);
            var distances = new double[_articles.Count];
            for (int i = 0; i < _articles.Count; i++)
            {
                var firstVector = new List<double>();
                var secondVector = new List<double>();
                foreach (var distinctWord in _distinctWords)
                {
                    firstVector.Add(_weights[distinctWord][i]);
                    secondVector.Add(_weights[distinctWord][articleIndex]);
                }

                distances[i] = CurrentMetric.GetDistance(firstVector, secondVector);
            }

            return distances.Zip(_articles, (d, a) => (a, d)).ToList();
        }

        private double GetFrequency(Article article, string word)
        {
            return article.Words.Count(s => s.Equals(word, StringComparison.CurrentCultureIgnoreCase));
        }
    }
}
