using CharMax.Helper;
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

        public Data(DataTable dataSet)
        {
            this.DataSet = dataSet;
            this.Decisions = FindDecisions();
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
