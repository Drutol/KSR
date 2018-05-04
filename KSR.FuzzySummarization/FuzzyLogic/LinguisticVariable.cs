using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using KSR.FuzzySummarization.Interfaces;
using KSR.FuzzySummarization.Model;
using Newtonsoft.Json;

namespace KSR.FuzzySummarization
{
    public class LinguisticVariable
    {
        public string Name { get; set; }
        public Type MembershipFunctionType { get; set; }
        public List<double> MembershipFunctionParameters { get; set; }
        public string MemberToExtract { get; set; }

        [JsonIgnore] public IMembershipFunction MembershipFunction { get; set; }
        [JsonIgnore] public Func<DataRecord, double> Extractor { get; set; }
        [JsonIgnore] public bool IsQuantifier { get; set; }

        [OnDeserialized]
        internal void OnSerializingMethod(StreamingContext context)
        {
            MembershipFunction = (IMembershipFunction) Activator.CreateInstance(MembershipFunctionType);
            MembershipFunction.Parameters = MembershipFunctionParameters;
            if (MemberToExtract != null)
            {
                var getterMethodInfo = typeof(DataRecord).GetProperty(MemberToExtract).GetGetMethod();
                var entity = Expression.Parameter(typeof(DataRecord));
                var getterCall = Expression.Call(entity, getterMethodInfo);
                var castToObject = Expression.Convert(getterCall, typeof(double));
                var lambda = Expression.Lambda(castToObject, entity);

                Extractor = (Func<DataRecord, double>) lambda.Compile();
            }
            else
            {
                IsQuantifier = true;
            }
        }
    }
}
