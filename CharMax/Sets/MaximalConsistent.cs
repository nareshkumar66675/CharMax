﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CharMax.Sets
{
    class MaximalConsistent
    {
        public List<List<int>> BMaximal { get; set; } = new List<List<int>>();

        public Dictionary<int, List<List<int>>> AConsistent { get; set; } = new Dictionary<int, List<List<int>>>();

        public void SetMaximalConsistents(Characteristic characteristic)
        {
            List<List<int>> tempBMaximal = new List<List<int>>();
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
                tempBMaximal.Add(blocks);
            }

            BMaximal = FindAndRemoveDuplicateMaximalBlocks(tempBMaximal);
            SetAConsistent(characteristic.CharacteristicSets.Count);
        }

        private void SetAConsistent(int count)
        {
            for (int i = 1; i <= count; i++)
            {
                AConsistent.Add(i, BMaximal.Where(t => t.Contains(i)).ToList());
            }
        }

        private List<List<int>> FindAndRemoveDuplicateMaximalBlocks(List<List<int>> bMaximal)
        {
            var singleBlocks = bMaximal.Where(t => t.Count == 1).ToList();

            foreach (var single in singleBlocks)
            {
                if(bMaximal.Where(t => t.Contains(single.FirstOrDefault()) && t.Count != 1).Any())
                {
                    bMaximal.RemoveAll(t => t.Contains(single.FirstOrDefault()) && t.Count == 1);
                }
            }
            return bMaximal;

        }

        private bool CheckSubset(List<int> SuperSet, List<int> SubSet)
        {
            return !SubSet.Except(SuperSet).Any();
        }
    }
}
