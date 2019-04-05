using CharMax.Helper;
using CharMax.Models;
using CharMax.Operations;
using CharMax.Sets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CharMax
{
    class Program
    {
        static void Main(string[] args)
        {
            var dataTable = FileOperation.ReadDataFile(@"C:\Users\kumar\OneDrive\Documents\Projects\CharMax\CharMax\Datasets\lymphography-35.d");

            Data data = new Data(dataTable);

            var conceptApprox = ProbApprox.GetConceptApprox(data.Characteristic, data.ConditionalProbability.CharacteristicCondProb, (float)2/(float)4);

            var mcbApprox = ProbApprox.GetConceptApprox(data.MaximalConsistent, data.ConditionalProbability.MaximalCondProb, (float)4 / (float)40);

            RuleInduction ruleInduction = new RuleInduction();

            var rules = ruleInduction.ComputeRules(data, data.AttributeValuePairs, mcbApprox);

            PrintRules(rules);
        }

        static void PrintRules(List<KeyValuePair<string, Rule>> Rules)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in Rules)
            {
                var temp = item.Value.Rules.Select(t => "(" + t.AttributeValue.Attribute + "," + t.AttributeValue.Value + ")");
                sb.AppendLine($"{string.Join(",", temp.ToList())} ---------- {item.Key}");
            }

            Console.Write(sb.ToString());
        }
    }
}
