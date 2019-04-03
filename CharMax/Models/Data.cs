using CharMax.Helper;
using CharMax.Operations;
using CharMax.Sets;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CharMax.Models
{
    class Data
    {
        public Dictionary<string, List<int>> Decisions { get; set; } = new Dictionary<string, List<int>>();
        public DataTable DataSet { get; set; } = new DataTable();
        public List<AttributeValuePair> AttributeValuePairs { get; set; } = new List<AttributeValuePair>();
        public Characteristic Characteristic { get; set; } 
        public MaximalConsistent MaximalConsistent { get; set; }
        public ConditionalProbability ConditionalProbability { get; set; }

        public Data(DataTable dataSet)
        {
            this.DataSet = dataSet;
            this.Decisions = FindDecisions();
            AttributeValuePairs = AttributeValuePair.GetAttributeValuePairs(dataSet);
            Characteristic = new Characteristic(this);
            MaximalConsistent = new MaximalConsistent(this);
            ConditionalProbability = ProbApprox.GetConditionalProbability(this);
        }

        private Dictionary<string, List<int>> FindDecisions()
        {
            Dictionary<string, List<int>> decisions = new Dictionary<string, List<int>>();

            int decisionOrdinal = DataSet.Columns.Count - 2;

            var distinctValues = DataSet.FindDistinctValues<string>(decisionOrdinal);

            var dataEnumer = DataSet.AsEnumerable();

            foreach (var value in distinctValues)
            {
                var concepts = dataEnumer.FindRecords(DataSet.Columns[decisionOrdinal], value);
                decisions.Add(value, concepts);
            }

            return decisions;
        }
    }
}
