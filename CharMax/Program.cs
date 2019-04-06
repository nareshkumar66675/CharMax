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
            var dataTable = FileOperation.ReadDataFile(@"C:\Users\kumar\OneDrive\Documents\Projects\CharMax\CharMax\Datasets\breast-35.d");

            Data data = new Data(dataTable);

            var charApprox = ProbApprox.GetConceptApprox(data.Characteristic, data.ConditionalProbability.CharacteristicCondProb, (float)6/(float)10);

            var mcbApprox = ProbApprox.GetConceptApprox(data.MaximalConsistent, data.ConditionalProbability.MaximalCondProb, (float)6 / (float)10);

            RuleInduction ruleInductionCharacteristic = new RuleInduction();

            RuleInduction ruleInductionMCB = new RuleInduction();

            var charTask = Task.Factory.StartNew(()=> ruleInductionCharacteristic.ComputeRules(data, data.AttributeValuePairs, charApprox));
            var mcBTask = Task.Factory.StartNew(() => ruleInductionMCB.ComputeRules(data, data.AttributeValuePairs, mcbApprox));

            Task.WaitAll(charTask,mcBTask);

            var charRule = charTask.Result;
            var mcbRule = mcBTask.Result;
            Console.WriteLine("Rules - Characteristic Set");
            PrintRules(charRule);

            Console.WriteLine("\n\n\n End of Characteristic Rules \n \n");
            Console.WriteLine("Rules - MCB");
            PrintRules(mcbRule);
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
