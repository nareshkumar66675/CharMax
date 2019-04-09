using CharMax.Helper;
using CharMax.Models;
using CharMax.Operations;
using CharMax.Sets;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CharMax
{
    class Program
    {
        static void Main(string[] args)
        {
            //var dataTable = FileOperation.ReadDataFile(@"C:\Users\kumar\OneDrive\Documents\Projects\CharMax\CharMax\Datasets\breast-35.d");

            var files = Directory.GetFiles(ConfigurationManager.AppSettings["DataSetFolder"],"*.d",SearchOption.TopDirectoryOnly);
            ConcurrentBag<Rules> AllRules = new ConcurrentBag<Rules>();

            //Parallel.ForEach(files, file =>
            //{
            //    var dataTable = FileOperation.ReadDataFile(file);
            //    Data data = new Data(dataTable);
            //    var alphaValue = 0.0f;
            //    Parallel.For(0, 12, alpha =>
            //    {
            //        alphaValue = (float)Math.Round(alphaValue + 0.10f, 1);
            //        RuleInduction ruleInductionCharacteristic = new RuleInduction();

            //        RuleInduction ruleInductionMCB = new RuleInduction();
            //        var charTask = Task.Factory.StartNew(() =>
            //        {
            //            var charApprox = ProbApprox.GetConceptApprox(data.Characteristic, data.ConditionalProbability.CharacteristicCondProb, alphaValue);
            //            return ruleInductionCharacteristic.ComputeRules(data, data.AttributeValuePairs, charApprox);
            //        });

            //        var mcBTask = Task.Factory.StartNew(() =>
            //        {
            //            var mcbApprox = ProbApprox.GetConceptApprox(data.MaximalConsistent, data.ConditionalProbability.MaximalCondProb, alphaValue);
            //            return ruleInductionMCB.ComputeRules(data, data.AttributeValuePairs, mcbApprox);
            //        });

            //        Task.WaitAll(charTask, mcBTask);

            //        var charRule = charTask.Result;
            //        var mcbRule = mcBTask.Result;

            //        Rules rules = new Rules(Path.GetFileNameWithoutExtension(file), alphaValue, charRule, mcbRule);

            //        AllRules.Add(rules);
            //    });

            //for (float alphaValue = 0.0f; alphaValue <= 1.0f; alphaValue = (float)Math.Round(alphaValue + 0.10f, 1))
            //{
            //    //RuleInduction ruleInductionCharacteristic = new RuleInduction();

            //    //RuleInduction ruleInductionMCB = new RuleInduction();
            //    //var charTask = Task.Factory.StartNew(() =>
            //    //{
            //    //    var charApprox = ProbApprox.GetConceptApprox(data.Characteristic, data.ConditionalProbability.CharacteristicCondProb, alphaValue);
            //    //    return ruleInductionCharacteristic.ComputeRules(data, data.AttributeValuePairs, charApprox);
            //    //});

            //    //var mcBTask = Task.Factory.StartNew(() =>
            //    //{
            //    //    var mcbApprox = ProbApprox.GetConceptApprox(data.MaximalConsistent, data.ConditionalProbability.MaximalCondProb, alphaValue);
            //    //    return ruleInductionMCB.ComputeRules(data, data.AttributeValuePairs, mcbApprox);
            //    //});

            //    //Task.WaitAll(charTask, mcBTask);

            //    //var charRule = charTask.Result;
            //    //var mcbRule = mcBTask.Result;

            //    //Rules rules = new Rules(Path.GetFileNameWithoutExtension(file), alphaValue, charRule, mcbRule);

            //    //AllRules.Add(rules);

            //}
            //});

            foreach (var file in files)
            {
                var dataTable = FileOperation.ReadDataFile(file);
                Data data = new Data(dataTable);

                for (float alphaValue = 0.0f; alphaValue <= 1.0f; alphaValue = (float)Math.Round(alphaValue + 0.10f, 1))
                {
                    RuleInduction ruleInductionCharacteristic = new RuleInduction();

                    RuleInduction ruleInductionMCB = new RuleInduction();
                    var charTask = Task.Factory.StartNew(() =>
                    {
                        var charApprox = ProbApprox.GetConceptApprox(data.Characteristic, data.ConditionalProbability.CharacteristicCondProb, alphaValue);
                        return ruleInductionCharacteristic.ComputeRules(data, data.AttributeValuePairs, charApprox);
                    });

                    var mcBTask = Task.Factory.StartNew(() =>
                    {
                        var mcbApprox = ProbApprox.GetConceptApprox(data.MaximalConsistent, data.ConditionalProbability.MaximalCondProb, alphaValue);
                        return ruleInductionMCB.ComputeRules(data, data.AttributeValuePairs, mcbApprox);
                    });

                    Task.WaitAll(charTask, mcBTask);

                    var charRule = charTask.Result;
                    var mcbRule = mcBTask.Result;

                    Rules rules = new Rules(Path.GetFileNameWithoutExtension(file), alphaValue, charRule, mcbRule);

                    AllRules.Add(rules);

                }
            }


            RuleStat ruleStat = new RuleStat();

            ruleStat.SaveRuleStat(AllRules.ToList());

            //Console.WriteLine("Rules - Characteristic Set");
            //PrintRules(charRule);

            //Console.WriteLine("\n\n\n End of Characteristic Rules \n \n");
            //Console.WriteLine("Rules - MCB");
            //PrintRules(mcbRule);
        }

        static void PrintRules(List<KeyValuePair<string, Rule>> Rules)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in Rules)
            {
                var temp = item.Value.Rules.Select(t => "(" + t.AttributeValue.Attribute + "," + t.AttributeValue.Value + ")");
                sb.AppendLine($"{string.Join(",", temp.ToList())} ---------- {item.Key}");
            }

            Console.Write(sb.ToString());
        }
    }
}
