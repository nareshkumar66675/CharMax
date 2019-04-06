using CharMax.Models;
using CharMax.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CharMax.Operations
{
    class RuleInduction
    {
        public List<KeyValuePair<string, Rule>> ComputeRules(Data data, List<AttributeValuePair> attributeValuePairs, Dictionary<string, List<int>> probApprox)
        {
            List<KeyValuePair<string, Rule>> computedRules = new List<KeyValuePair<string, Rule>>();
            foreach (var probBlocks in probApprox)
            {
                Rule rule;
                List<int> decisionBlock;
                data.Decisions.TryGetValue(probBlocks.Key, out decisionBlock);
                var itrProbBlocks = probBlocks;
                List<int> covered = new List<int>();
                while (true)
                {
                    rule = RecursiveRuleInduction(data, attributeValuePairs, itrProbBlocks, null, probBlocks);
                    if (rule != null)
                    {
                        computedRules.Add(new KeyValuePair<string, Rule>(probBlocks.Key, rule));

                        covered.AddRange(rule.Covers);

                        

                        var remaining = probBlocks.Value.Except(covered.Distinct()).ToList();

                        itrProbBlocks = new KeyValuePair<string, List<int>>(probBlocks.Key, remaining);

                        if (remaining.Count == 0)
                        {
                            break;
                        }
                    }
                    else
                        break;
                }

            }

            return DropRules(computedRules);
        }




        public Rule RecursiveRuleInduction(Data data, List<AttributeValuePair> attributeValuePairs, 
            KeyValuePair<string,List<int>> probBlocks, Rule tempRules, KeyValuePair<string, List<int>> originalProbBlocks)
        {

            if (attributeValuePairs.Count(t => t.Blocks.Count == 0) == attributeValuePairs.Count)
                return null;

            List<AttributeValuePair> tempMatchingBlocks = new List<AttributeValuePair>();

            foreach (var attributeValuePair in attributeValuePairs)
            {
                var matchAttrBlock = attributeValuePair.Blocks.Intersect(probBlocks.Value).ToList();

                AttributeValuePair tempMatchAttrPair = new AttributeValuePair();
                tempMatchAttrPair.AttributeValue.Attribute = attributeValuePair.AttributeValue.Attribute;
                tempMatchAttrPair.AttributeValue.Value = attributeValuePair.AttributeValue.Value;
                tempMatchAttrPair.Blocks = matchAttrBlock;

                tempMatchingBlocks.Add(tempMatchAttrPair);
            }

            //Get Maximum Count in matching block values
            var maxCountMatchBlock =tempMatchingBlocks.OrderByDescending(maxCountBlock => maxCountBlock.Blocks.Count).FirstOrDefault().Blocks.Count();

            //Find the blocks based on the maximum count
            var maxMatchingBlocks = tempMatchingBlocks.Where(maxCountBlock => maxCountBlock.Blocks.Count == maxCountMatchBlock).ToList();

            //Find the attribute value pair
            var selectedAttrValPair =  data.AttributeValuePairs.Where(mainAttrValPair => maxMatchingBlocks.Exists(matchAttrPair => matchAttrPair.AttributeValue.Attribute == mainAttrValPair.AttributeValue.Attribute
                && matchAttrPair.AttributeValue.Value == mainAttrValPair.AttributeValue.Value)).OrderBy(maxCount => maxCount.Blocks.Count).First();

            data.Decisions.TryGetValue(probBlocks.Key, out List<int> decision);

            //First Iteration
            if (tempRules == null)
            {
                //Check if the attribute is subset - FIRST Iteration
                if(originalProbBlocks.Value.CheckSubset(selectedAttrValPair.Blocks))
                {
                    Rule rule = new Rule();
                    rule.Decision = data.Decisions.Where(t => t.Key == probBlocks.Key).First();
                    rule.Rules.Add(selectedAttrValPair);

                    rule.Covers = originalProbBlocks.Value.Intersect(selectedAttrValPair.Blocks).ToList();

                    return rule;
                }
                // If Not a Subset add it to the rules for next iterations
                else
                {
                    tempRules = new Rule();
                    tempRules.Decision = data.Decisions.Where(t => t.Key == probBlocks.Key).First();
                    tempRules.Rules.Add(selectedAttrValPair);

                    var nextItrProbBlocks = new List<int>(maxMatchingBlocks.Where(t => t.AttributeValue == selectedAttrValPair.AttributeValue).First().Blocks);
                    tempMatchingBlocks.Where(t => t.AttributeValue == selectedAttrValPair.AttributeValue).First().Blocks = new List<int>();


                    return RecursiveRuleInduction(data, tempMatchingBlocks, new KeyValuePair<string, List<int>>(tempRules.Decision.Key, nextItrProbBlocks),  tempRules,originalProbBlocks);
                }
            }
            else
            {
                tempRules.Rules.Add(selectedAttrValPair);

                //var newUnionBlock = tempRules.Rules.Select(t => t.Blocks).IntersectAll().ToList();
                var rules = CheckSubsets(tempRules, originalProbBlocks);
                if(rules!=null)
                {
                    tempRules.Covers = originalProbBlocks.Value.Intersect(rules.Select(t=>t.Blocks).IntersectAll()).ToList();
                    tempRules.Rules = rules;
                    return tempRules;
                }
                else
                {
                    //Unsuccessfull next iteration - 
                    var nextItrProbBlocks = new List<int>(maxMatchingBlocks.Where(t => t.AttributeValue == selectedAttrValPair.AttributeValue).First().Blocks);
                    tempMatchingBlocks.Where(t => t.AttributeValue == selectedAttrValPair.AttributeValue).First().Blocks = new List<int>();

                    return RecursiveRuleInduction(data, tempMatchingBlocks, new KeyValuePair<string, List<int>>(tempRules.Decision.Key, nextItrProbBlocks), tempRules,originalProbBlocks);
                }
            }



        }

        private List<KeyValuePair<string, Rule>> DropRules(List<KeyValuePair<string, Rule>> Rules)
        {
            List<KeyValuePair<string, Rule>> duplicateRules = new List<KeyValuePair<string, Rule>>();

            for (int i = 0; i < Rules.Count; i++)
            {
                for (int j = 0; j < Rules.Count; j++)
                {
                    if (i == j)
                        continue;
                    if(Rules[j].Value.Covers.CheckSubset(Rules[i].Value.Covers))
                    {
                        duplicateRules.Add(Rules[i]);
                        break;
                    }
                }
            }

            return Rules;
        }

        private List<AttributeValuePair> CheckSubsets(Rule tempRules, KeyValuePair<string, List<int>> originalProbBlocks)
        {
            for (int i = 0; i < tempRules.Rules.Count; i++)
            {
                if (originalProbBlocks.Value.CheckSubset(tempRules.Rules[i].Blocks))
                {
                    List<AttributeValuePair> rul = new List<AttributeValuePair>();
                    rul.Add(tempRules.Rules[i]);
                    return rul;
                }

            }

            bool isSubset = false;
            List<AttributeValuePair> rules = new List<AttributeValuePair>();
            rules.Add(tempRules.Rules[0]);
            for (int i = 1; i < tempRules.Rules.Count; i++)
            {
                for (int j = i; j < tempRules.Rules.Count; j++)
                {
                    var tempArr = new AttributeValuePair[rules.Count];
                    rules.CopyTo(tempArr);
                    var tempList = tempArr.ToList();
                    tempList.Add(tempRules.Rules[j]);
                    var newIntersectBlock = tempList.Select(t => t.Blocks).IntersectAll().ToList() ;

                    if (originalProbBlocks.Value.CheckSubset(newIntersectBlock))
                    {
                        isSubset = true;
                        break;
                    }
                }
                rules.Add(tempRules.Rules[i]);
            }

            if (isSubset)
                return rules;
            else
                return null;
        }

        //private 
    }
}
