using CharMax.Models;
using CharMax.Helper:
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CharMax.Operations
{
    class RuleInduction
    {
        public void ComputeRules(List<AttributeValuePair> attributeValuePairs, Dictionary<string, List<int>> probApprox)
        {
            foreach (var probBlocks in probApprox)
            {



            }
        }

        public Rule RecursiveRuleInduction(List<AttributeValuePair> attributeValuePairs, KeyValuePair<string,List<int>> probBlocks,Data data,
            Rule tempRules)
        {
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
            var selectedAttrValPair =  attributeValuePairs.Where(mainAttrValPair => maxMatchingBlocks.Exists(matchAttrPair => matchAttrPair.AttributeValue.Attribute == mainAttrValPair.AttributeValue.Attribute
                && matchAttrPair.AttributeValue.Value == mainAttrValPair.AttributeValue.Value)).OrderBy(maxCount => maxCount.Blocks.Count).First();

            data.Decisions.TryGetValue(probBlocks.Key, out List<int> decision);

            //First Iteration
            if (tempRules == null)
            {
                //Check if the attribute is subset - FIRST Iteration
                if(decision.CheckSubset(selectedAttrValPair.Blocks))
                {
                    Rule rule = new Rule();
                    rule.Decision = data.Decisions.Where(t => t.Key == probBlocks.Key).First();
                    rule.Rules.Add(selectedAttrValPair);

                    return rule;
                }
                // If Not a Subset add it to the rules for next iterations
                else
                {
                    tempRules = new Rule();
                    tempRules.Decision = data.Decisions.Where(t => t.Key == probBlocks.Key).First();
                    tempRules.Rules.Add(selectedAttrValPair);
                }
            }
            else
            {
                tempRules.Rules.Add(selectedAttrValPair);

                var newUnionBlock = tempRules.Rules.Select(t => t.Blocks).IntersectAll().ToList();

                if(tempRules.Decision.Value.CheckSubset(newUnionBlock))
                {
                    return tempRules;
                }
                else
                {
                    return RecursiveRuleInduction(tempMatchingBlocks, new KeyValuePair<string, List<int>>(, data, tempRules);
                }
            }



        }


        //private bool CheckSubsets(Rule tempRules)
        //{
        //    for (int i = 0; i < tempRules.Rules.Count; i++)
        //    {

        //    }
        //}

        //private 
    }
}
