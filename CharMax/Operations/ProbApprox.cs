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
        public Dictionary<KeyValuePair<string, List<int>>, Dictionary<int, float>> Probabilities { get; set; } 
            = new Dictionary<KeyValuePair<string, List<int>>, Dictionary<int, float>>();

        public void SetConceptProbabilities(DataTable dataSet, Characteristic characteristic)
        {
            var decisions = FindDecisions(dataSet);

            foreach (var decision in decisions)
            {
                Dictionary<int, float> tempProbs = new Dictionary<int, float>();
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
        private float FindProbability(List<int> SetA, List<int> SetB)
        {
            return (float)SetA.Intersect(SetB).Count() / (float)SetA.Count;
        }


        private Dictionary<string,List<int>> FindDecisions(DataTable dataSet)
        {
            Dictionary<string, List<int>> decisions = new Dictionary<string, List<int>>();

            int decisionOrdinal = dataSet.Columns.Count - 2;

            var distinctValues = dataSet.FindDistinctValues<string>(decisionOrdinal);

            var dataEnumer = dataSet.AsEnumerable();

            foreach (var value in distinctValues)
            {
                var concepts = dataEnumer.FindRecords(dataSet.Columns[decisionOrdinal],value);
                decisions.Add(value, concepts);
            }

            return decisions;
        }
    }
}
