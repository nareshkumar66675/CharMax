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
                    rule.Conditions.Add(selectedAttrValPair);

                    rule.Covers = originalProbBlocks.Value.Intersect(selectedAttrValPair.Blocks).ToList();

                    return rule;
                }
                // If Not a Subset add it to the rules for next iterations
                else
                {
                    tempRules = new Rule();
                    tempRules.Decision = data.Decisions.Where(t => t.Key == probBlocks.Key).First();
                    tempRules.Conditions.Add(selectedAttrValPair);

                    var nextItrProbBlocks = new List<int>(maxMatchingBlocks.Where(t => t.AttributeValue == selectedAttrValPair.AttributeValue).First().Blocks);
                    var nullify = tempMatchingBlocks.Where(t => t.AttributeValue == selectedAttrValPair.AttributeValue).First();
                    nullify.Blocks = new List<int>();

                    return RecursiveRuleInduction(data, tempMatchingBlocks, new KeyValuePair<string, List<int>>(tempRules.Decision.Key, nextItrProbBlocks),  tempRules,originalProbBlocks);
                }
            }
            else
            {
                tempRules.Conditions.Add(selectedAttrValPair);

                //var newUnionBlock = tempRules.Rules.Select(t => t.Blocks).IntersectAll().ToList();
                var rules = CheckSubsets(tempRules, originalProbBlocks);
                if(rules!=null)
                {
                    tempRules.Covers = originalProbBlocks.Value.Intersect(rules.Select(t=>t.Blocks).IntersectAll()).ToList();
                    tempRules.Conditions = rules;
                    return tempRules;
                }
                else
                {
                    //Unsuccessfull next iteration - 
                    var nextItrProbBlocks = new List<int>(maxMatchingBlocks.Where(t => t.AttributeValue == selectedAttrValPair.AttributeValue).First().Blocks);
                    var nullify = tempMatchingBlocks.Where(t => t.AttributeValue == selectedAttrValPair.AttributeValue).First();
                    nullify.Blocks = new List<int>();
                    return RecursiveRuleInduction(data, tempMatchingBlocks, new KeyValuePair<string, List<int>>(tempRules.Decision.Key, nextItrProbBlocks), tempRules,originalProbBlocks);
                }
            }

        }

        private List<KeyValuePair<string, Rule>> DropRules(List<KeyValuePair<string, Rule>> Rules)
        {
            List<KeyValuePair<string, Rule>> duplicateRules = new List<KeyValuePair<string, Rule>>();

            var tempRules = Rules.OrderBy(t => t.Value.Covers.Count).ToList();

            for (int i = 0; i < tempRules.Count; i++)
            {
                for (int j = 0; j < tempRules.Count; j++)
                {
                    if (i == j)
                        continue;
                    if(tempRules[j].Value.Covers.CheckSubset(tempRules[i].Value.Covers))
                    {
                        duplicateRules.Add(tempRules[i]);
                        break;
                    }
                }
            }

            if(Rules.Count != duplicateRules.Count)
            foreach (var item in duplicateRules)
            {
                    Rules.Remove(item);
            }
            return DropConditions(Rules);
        }

        private List<KeyValuePair<string, Rule>> DropConditions(List<KeyValuePair<string, Rule>> Rules)
        {
            foreach (var rule in Rules)
            {
                var droppedConditions = CheckSubsets(rule.Value, new KeyValuePair<string, List<int>>(rule.Key, rule.Value.Covers),true);
                //for (int i = 0; i < rule.Value.Conditions.Count; i++)
                //{

                //}
                rule.Value.Conditions = droppedConditions ?? rule.Value.Conditions;

            }

            return Rules;
        }

        private List<AttributeValuePair> CheckSubsets(Rule tempRules, KeyValuePair<string, List<int>> originalProbBlocks,bool conditionCheck = false)
        {
            for (int i = 0; i < tempRules.Conditions.Count; i++)
            {
                if (originalProbBlocks.Value.CheckSubset(tempRules.Conditions[i].Blocks))
                {
                    List<AttributeValuePair> rul = new List<AttributeValuePair>();
                    rul.Add(tempRules.Conditions[i]);
                    return rul;
                }

            }

            bool isSubset = false;
            List<AttributeValuePair> rules = new List<AttributeValuePair>();
            rules.Add(tempRules.Conditions[0]);
            for (int i = 1; i < tempRules.Conditions.Count; i++)
            {
                for (int j = i; j < tempRules.Conditions.Count; j++)
                {
                    var tempArr = new AttributeValuePair[rules.Count];
                    rules.CopyTo(tempArr);
                    var tempList = tempArr.ToList();
                    tempList.Add(tempRules.Conditions[j]);
                    var newIntersectBlock = tempList.Select(t => t.Blocks).IntersectAll().ToList() ;

                    if (originalProbBlocks.Value.CheckSubset(newIntersectBlock))
                    {
                        isSubset = true;
                        break;
                    }

                }

                rules.Add(tempRules.Conditions[i]);
                if (isSubset)
                    break;
            }

            if (isSubset)
                return rules;
            else
                return null;
        }

        //private 
    }
}
