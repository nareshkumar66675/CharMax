using CharMax.Helper;
using CharMax.Models;
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

        public ConditionalProbability GetConditionalProbability(Data data)
        {
            ConditionalProbability conditionalProbability = new ConditionalProbability();

            var charTask = Task.Factory.StartNew(() => ComputeConditionalProbability(data,data.Characteristic));

            var maxTask = Task.Factory.StartNew(() => ComputeConditionalProbability(data, data.MaximalConsistent));

            charTask.Wait();
            maxTask.Wait();

            conditionalProbability.CharacteristicCondProb = charTask.Result;
            conditionalProbability.MaximalCondProb = maxTask.Result;

            return conditionalProbability;
        }

        /// <summary>
        /// (Decision-value,{1,2,3,4}),{(1,0.5),(2,0.667),(3,1)}
        /// </summary>
        //public Dictionary<KeyValuePair<string, List<int>>, Dictionary<int, float>> Probabilities { get; set; } 
        //    = new Dictionary<KeyValuePair<string, List<int>>, Dictionary<int, float>>();

        //List<Probabilities> Probabilities { get; set; } = new List<Probabilities>();

        public List<Probabilities<int>> ComputeConditionalProbability(Data data, Characteristic characteristic)
        {
            List<Probabilities<int>> conditionalProb = new List<Probabilities<int>>();

            foreach (var decision in data.Decisions)
            {
                Dictionary<int, float> tempProbs = new Dictionary<int, float>();
                foreach (var characteristicValues in characteristic.CharacteristicSets)
                {
                    tempProbs.Add(characteristicValues.Key, FindProbability(characteristicValues.Value, decision.Value));
                }
                Probabilities<int> tempProbabilities = new Probabilities<int>
                {
                    Decision = decision.Key,
                    DecisionSet = decision.Value,
                    ConditionalProb = tempProbs
                };
                conditionalProb.Add(tempProbabilities);
            }

            return conditionalProb;
        }

        public List<Probabilities<List<int>>> ComputeConditionalProbability(Data data, MaximalConsistent maximalConsistent)
        {
            List<Probabilities<List<int>>> conditionalProb = new List<Probabilities<List<int>>>();

            foreach (var decision in data.Decisions)
            {
                Dictionary<List<int>, float> tempProbs = new Dictionary<List<int>, float>();
                foreach (var maximalBlocks in maximalConsistent.MaximalBlocks)
                {
                    tempProbs.Add(maximalBlocks, FindProbability(maximalBlocks, decision.Value));
                }
                Probabilities<List<int>> tempProbabilities = new Probabilities<List<int>>
                {
                    Decision = decision.Key,
                    DecisionSet = decision.Value,
                    ConditionalProb = tempProbs
                };
                conditionalProb.Add(tempProbabilities);
            }

            return conditionalProb;
        }

        public Dictionary<string,List<int>> GetConceptApprox(Characteristic characteristic, List<Probabilities<int>> conditionalProb, float alpha)
        {
            Dictionary<string, List<int>> tempApprox = new Dictionary<string, List<int>>();
            foreach (var prob in conditionalProb)
            {
                var charIds = prob.ConditionalProb.Where(t => t.Value >= alpha).Select(u => u.Key).ToList();

                // Union all Characteristic Sets
                var approx =  characteristic.CharacteristicSets.Where(set => charIds.Contains(set.Key)).SelectMany(u => u.Value).Distinct().ToList();
                approx.Sort();
                tempApprox.Add(prob.Decision, approx);
            }

            return tempApprox;
        }

        public Dictionary<string, List<int>> GetConceptApprox(MaximalConsistent maximalConsistent, List<Probabilities<List<int>>> conditionalProb, float alpha)
        {
            Dictionary<string, List<int>> tempApprox = new Dictionary<string, List<int>>();
            foreach (var prob in conditionalProb)
            {
                var charIds = prob.ConditionalProb.Where(t => t.Value >= alpha).Select(u => u.Key).ToList();

                // Union all Characteristic Sets
                var approx = charIds.SelectMany(u => u).Distinct().ToList(); //var approx = characteristic.CharacteristicSets.Where(set => charIds.Contains(set.Key)).SelectMany(u => u.Value).Distinct().ToList();
                approx.Sort();
                tempApprox.Add(prob.Decision, approx);
            }

            return tempApprox;
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



    }
}
