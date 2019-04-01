using CharMax.Helper;
using CharMax.Sets;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CharMax.Operations
{
    class ProbApprox
    {
        /// <summary>
        /// (Decision-value,{1,2,3,4}),{(1,0.5),(2,0.667),(3,1)}
        /// </summary>
        public Dictionary<KeyValuePair<string, List<int>>, Dictionary<int, int>> Probabilities { get; set; } 
            = new Dictionary<KeyValuePair<string, List<int>>, Dictionary<int, int>>();

        public void SetConceptProbabilities(DataTable dataSet, Characteristic characteristic)
        {
            var decisions = FindDecisions(dataSet);

            foreach (var decision in decisions)
            {
                Dictionary<int, int> tempProbs = new Dictionary<int, int>();
                foreach (var characteristicValues in characteristic.CharacteristicSets)
                {
                    tempProbs.Add(characteristicValues.Key, FindProbability(characteristicValues.Value, decision.Value));
                }
                Probabilities.Add(decision, tempProbs);
            }
        }

        /// <summary>
        /// (|SetA Intersection SetB|)/SetA
        /// </summary>
        /// <param name="SetA"></param>
        /// <param name="SetB"></param>
        /// <returns></returns>
        private int FindProbability(List<int> SetA, List<int> SetB)
        {
            return SetA.Intersect(SetB).Count() / SetA.Count;
        }


        private Dictionary<string,List<int>> FindDecisions(DataTable dataSet)
        {
            Dictionary<string, List<int>> decisions = new Dictionary<string, List<int>>();

            var distinctValues = dataSet.FindDistinctValues<string>(dataSet.Columns.Count - 1);

            var dataEnumer = dataSet.AsEnumerable();

            int decisionOrdinal = dataSet.Columns.Count - 1;

            foreach (var value in distinctValues)
            {
                var concepts = dataEnumer.FindRecords(dataSet.Columns[decisionOrdinal],value);
                decisions.Add(value, concepts);
            }

            return decisions;
        }
    }
}
