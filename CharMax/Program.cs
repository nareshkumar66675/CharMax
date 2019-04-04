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
            var dataTable = FileOperation.ReadDataFile(@"C:\Users\kumar\OneDrive\Documents\Projects\CharMax\CharMax\Datasets\test.d");

            Data data = new Data(dataTable);

            //var conceptApprox = ProbApprox.GetConceptApprox(data.Characteristic, data.ConditionalProbability.CharacteristicCondProb, (float)4/(float)4);

            var mcbApprox = ProbApprox.GetConceptApprox(data.MaximalConsistent, data.ConditionalProbability.MaximalCondProb, (float)4 / (float)4);

            RuleInduction ruleInduction = new RuleInduction();

            ruleInduction.ComputeRules(data, data.AttributeValuePairs, mcbApprox);
        }
    }
}
