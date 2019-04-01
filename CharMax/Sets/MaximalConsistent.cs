using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CharMax.Sets
{
    class MaximalConsistent
    {
        public List<List<int>> BMaximal { get; set; } = new List<List<int>>();

        public List<List<int>> AConsistent { get; set; }

        public void SetBMaximalConsistent(Characteristic characteristic)
        {
            foreach (var set in characteristic.CharacteristicSets)
            {
                List<int> blocks = new List<int>();
                
                foreach (var el in set.Value)
                {
                    List<int> superset = new List<int>();
                    characteristic.CharacteristicSets.TryGetValue(el, out superset);
                    if(CheckSubset(superset, set.Value ))
                    {
                        blocks.Add(el);
                    }
                }
                BMaximal.Add(blocks);
            }
        }

        private bool CheckSubset(List<int> SuperSet, List<int> SubSet)
        {
            return !SubSet.Except(SuperSet).Any();
        }
    }
}
