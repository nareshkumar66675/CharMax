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

            ProbApprox probApprox = new ProbApprox();

            var condProb =probApprox.GetConditionalProbability(data);

            var conceptApprox = probApprox.GetConceptApprox(data.Characteristic, condProb.CharacteristicCondProb, (float)2/(float)3);

            var mcbApprox = probApprox.GetConceptApprox(data.MaximalConsistent, condProb.MaximalCondProb, (float)2 / (float)4);
        }
    }
}
