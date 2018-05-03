using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KSR.FuzzySummarization.Model;

namespace KSR.FuzzySummarization.DataProcessing
{
    public class DataExtractor
    {

        public IEnumerable<DataRecord> ObtainRecords()
        {
            var lines = File.ReadAllLines("Data/data.txt");
            foreach (var line in lines.Where(s => !s.Contains("?")))
            {
                var tokens = line.Split(new [] {','} ,StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList();
                if(!tokens.Any())
                    continue;
                yield return new DataRecord
                {
                    Age = int.Parse(tokens[0]),
                    Workclass = (Workclass) WorkClasses.IndexOf(tokens[1]),
                    SamplingWeight = int.Parse(tokens[2]),
                    Education = (Education) Educations.IndexOf(tokens[3]),
                    EductaionNumber = int.Parse(tokens[4]),
                    MaritalStatus = (MaritalStatus) MaritalStatuses.IndexOf(tokens[5]),
                    Occupation = (Occupation) Occupations.IndexOf(tokens[6]),
                    Relationship = (Relationship) Relationships.IndexOf(tokens[7]),
                    Race = (Race) Races.IndexOf(tokens[8]),
                    IsMale = tokens[9] == "Male",
                    CapitalGain = int.Parse(tokens[10]),
                    CapitalLoss = int.Parse(tokens[11]),
                    HoursPerWeek = int.Parse(tokens[12]),
                    NativeCountry = (NativeCountry) NativeCountries.IndexOf(tokens[13])
                };
            }
        }


        private List<string> WorkClasses = new List<string> { "Private", "Self-emp-not-inc", "Self-emp-inc", "Federal-gov", "Local-gov", "State-gov", "Without-pay", "Never-worked" };
        private List<string> Educations = new List<string> { "Bachelors", "Some-college", "11th", "HS-grad", "Prof-school", "Assoc-acdm", "Assoc-voc", "9th", "7th-8th", "12th", "Masters", "1st-4th", "10th", "Doctorate", "5th-6th", "Preschool" };
        private List<string> MaritalStatuses = new List<string> { "Married-civ-spouse", "Divorced", "Never-married", "Separated", "Widowed", "Married-spouse-absent", "Married-AF-spouse" };
        private List<string> Occupations = new List<string> { "Tech-support", "Craft-repair", "Other-service", "Sales", "Exec-managerial", "Prof-specialty", "Handlers-cleaners", "Machine-op-inspct", "Adm-clerical", "Farming-fishing", "Transport-moving", "Priv-house-serv", "Protective-serv", "Armed-Forces" };
        private List<string> Relationships = new List<string> { "Wife", "Own-child", "Husband", "Not-in-family", "Other-relative", "Unmarried" };
        private List<string> Races = new List<string> { "White", "Asian-Pac-Islander", "Amer-Indian-Eskimo", "Other", "Black" };
        private List<string> NativeCountries = new List<string> { "United-States", "Cambodia", "England", "Puerto-Rico", "Canada", "Germany", "Outlying-US(Guam-USVI-etc)", "India", "Japan", "Greece", "South", "China", "Cuba", "Iran", "Honduras", "Philippines", "Italy", "Poland", "Jamaica", "Vietnam", "Mexico", "Portugal", "Ireland", "France", "Dominican-Republic", "Laos", "Ecuador", "Taiwan", "Haiti", "Columbia", "Hungary", "Guatemala", "Nicaragua", "Scotland", "Thailand", "Yugoslavia", "El-Salvador", "Trinadad&Tobago", "Peru", "Hong", "Holand-Netherlands" };

    }
}
