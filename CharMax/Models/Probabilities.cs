using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CharMax.Models
{
    class Probabilities<T>
    {
        public string Decision { get; set; }

        public List<int> DecisionSet { get; set; }

        public Dictionary<T,float> ConditionalProb { get; set; }
    }

    class ConditionalProbability
    {
        public List<Probabilities<int>> CharacteristicCondProb { get; set; } = new List<Probabilities<int>>();

        public List<Probabilities<List<int>>> MaximalCondProb { get; set; } = new List<Probabilities<List<int>>>();

    }
}
