using CharMax.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CharMax.Helper
{
    public class RuleStat
    {
        public DataTable RuleStatistics { get; set; }

        public void SaveRuleStat(List<Rules> RuleSet)
        {
            DataTable dataSet = new DataTable();

            dataSet.Columns.Add("FileName", typeof(string));
            dataSet.Columns.Add("Alpha", typeof(float));
            dataSet.Columns.Add("CharRuleCount", typeof(int));
            dataSet.Columns.Add("MCBRuleCount", typeof(int));
            dataSet.Columns.Add("CharConditionCount", typeof(int));
            dataSet.Columns.Add("MCBConditionCount", typeof(int));

            foreach (var rule in RuleSet)
            {
                dataSet.Rows.Add(rule.FileName, rule.AlphaValue ==0.01f ?0.0f : rule.AlphaValue, rule.CharacteristicRules.Count, 
                    rule.MCBRules.Count,GetConditionCount(rule.CharacteristicRules),GetConditionCount(rule.MCBRules));
            }

            RuleStatistics = dataSet;

            RuleStatistics.WriteToCsvFile(ConfigurationManager.AppSettings["ResultFolder"]);
        }


        private int GetConditionCount(List<KeyValuePair<string, Models.Rule>> rules)
        {
            if (rules != null)
            {
                var attr = rules.Select(t => t.Value.Conditions.Select(u => u.AttributeValue.Attribute.Contains("|") ?
                    u.AttributeValue.Attribute.Substring(0, u.AttributeValue.Attribute.IndexOf("|") - 1) : u.AttributeValue.Attribute).Distinct().ToList());
                attr = attr.OrderBy(t => t.Count).Take(attr.Count() - 1);
                return attr.Sum(t => t.Count);
            }
            else
                return 0;
        }
    }
}
