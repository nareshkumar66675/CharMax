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

            AttributeValuePairs attributeValuePairs = new AttributeValuePairs();
            attributeValuePairs.Pairs = AttributeValuePairs.FindAttributeValuePairs(data);

            Characteristic characteristic = new Characteristic();

            characteristic.FindCharacteristicSets(data, attributeValuePairs);

            MaximalConsistent maximalConsistent = new MaximalConsistent();

            maximalConsistent.SetMaximalConsistents(characteristic);

            ProbApprox probApprox = new ProbApprox();

            var condProb =probApprox.GetConditionalProbability(data, characteristic, maximalConsistent);

            var conceptApprox = probApprox.GetConceptApprox(characteristic, condProb.CharacteristicCondProb, (float)2/(float)3);

            var mcbApprox = probApprox.GetConceptApprox(maximalConsistent, condProb.MaximalCondProb, (float)2 / (float)4);
        }
    }
}
