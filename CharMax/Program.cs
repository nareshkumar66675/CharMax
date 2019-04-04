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

            var conceptApprox = ProbApprox.GetConceptApprox(data.Characteristic, data.ConditionalProbability.CharacteristicCondProb, (float)6/(float)10);

            var mcbApprox = ProbApprox.GetConceptApprox(data.MaximalConsistent, data.ConditionalProbability.MaximalCondProb, (float)6 / (float)10);

            RuleInduction ruleInduction = new RuleInduction();

            ruleInduction.ComputeRules(data, data.AttributeValuePairs, mcbApprox);
        }
    }
}
