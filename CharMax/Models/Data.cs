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
    public enum ColumnType
    {
        SYMBOLIC,
        NUMERIC
    }
    class Data
    {
        public Dictionary<string, List<int>> Decisions { get; set; } = new Dictionary<string, List<int>>();
        public DataTable DataSet { get; set; } = new DataTable();
        public DataTable OriginalDataSet { get; set; } = new DataTable();
        public List<AttributeValuePair> AttributeValuePairs { get; set; } = new List<AttributeValuePair>();
        public Characteristic Characteristic { get; set; } 
        public MaximalConsistent MaximalConsistent { get; set; }
        public ConditionalProbability ConditionalProbability { get; set; }
        public Dictionary<string, ColumnType> ColumnTypes { get; set; } = new Dictionary<string, ColumnType>();
        public Dictionary<string, List<float>> CutPoints { get; set; } = new Dictionary<string, List<float>>();

        public Data(DataTable dataSet)
        {
            this.OriginalDataSet = dataSet;
            this.DataSet = ConvertNumericData(dataSet.Copy());
            this.Decisions = FindDecisions();
            AttributeValuePairs = AttributeValuePair.GetAttributeValuePairs(DataSet);
            Characteristic = new Characteristic(this);
            MaximalConsistent = new MaximalConsistent(this);
            ConditionalProbability = ProbApprox.GetConditionalProbability(this);
        }


        private DataTable ConvertNumericData(DataTable dataSet)
        {
            FindNumericOrSymbolic(dataSet);

            int decisionIndex = dataSet.Columns.Count - 2;
            foreach (var column in ColumnTypes)
            {
                var cutPoints = new List<float>();
                if(column.Value == ColumnType.NUMERIC)
                {

                    var distinctValuesString = dataSet.FindDistinctValues<string>(column.Key).Where(t => t != "?" && t != "*").ToList();

                    var distinctValues = distinctValuesString.Select(t => float.Parse(t)).ToList();

                    distinctValues.Sort();
                    var dataEnumer = dataSet.AsEnumerable();

                    var temp = string.Empty;

                    //var selectedValues = new List<float>();
                    //for (int i = 0; i < distinctValues.Count - 1; i++)
                    //{
                    //    var matchings = dataEnumer.Where(t => !(new string[] { "*", "?" }).Contains(t.Field<string>(column.Key)) && float.Parse(t.Field<string>(column.Key)) == distinctValues[i]).
                    //        Select(t => t.Field<string>(decisionIndex)).Distinct();

                    //    if (matchings.Count() > 1)
                    //        cutPoints.Add(distinctValues[i]);
                    //    else
                    //    {
                    //        if (temp == matchings.First())
                    //            continue;
                    //        else
                    //        {
                    //            temp = matchings.First();
                    //            selectedValues.Add(distinctValues[i]);
                    //        }
                    //    }
                    //}
                    //distinctValues = selectedValues;
                    int rnd = 4;

                    for (int i = 0; i < distinctValues.Count-1; i++)
                    {
                        cutPoints.Add((float)Math.Round((distinctValues[i] + distinctValues[i + 1]) / 2f, rnd));
                    }
                    cutPoints = cutPoints.Distinct().ToList();

                    CutPoints.Add(column.Key, cutPoints);


                    var min = distinctValues[0];
                    var max = distinctValues[distinctValues.Count - 1];


                    //Selective Cutpoint

                    
                    foreach (var cutPoint in cutPoints)
                    {
                        //var values = dataEnumer.Where(t => t.Field<string>(column.Key)!="*" && t.Field<float>(column.Key) >= min && t.Field<float>(column.Key) <= cutPoint);

                        //var values = from row in dataEnumer
                        //where row.Field<string>(column.Key) != "*" &&
                        //    float.Parse(row.Field<string>(column.Key)) >= min && float.Parse(row.Field<string>(column.Key))<=cutPoint
                                     //select row.Field<string>("ID");
                        
                        //var values = dataSet.Select($" {column.Key}<> '*' && Convert({column.Key}, 'System.Decimal') >={min} &&  Convert({column.Key}, 'System.Decimal') >={cutPoint}");
                    }


                    foreach (var cutPoint in cutPoints)
                    {
                        var newColumnName = string.Concat(column.Key,"|", cutPoint);
                        dataSet.Columns.Add(newColumnName, typeof(string));

                        foreach (DataRow row in dataSet.Rows)
                        {
                            var actualValue = row[column.Key].ToString();
                            if (actualValue == "?" || actualValue == "*")
                                row.SetField<string>(newColumnName, row[column.Key].ToString());
                            else if (Math.Round(float.Parse(actualValue), rnd) >= min && Math.Round(float.Parse(actualValue), rnd) <= cutPoint)
                                row.SetField<string>(newColumnName, $"{min}..{cutPoint}");
                            else if (Math.Round(float.Parse(actualValue), rnd) >= cutPoint && Math.Round(float.Parse(actualValue), rnd) <= max)
                                row.SetField<string>(newColumnName, $"{cutPoint}..{max}");
                            else
                                row.SetField<string>(newColumnName, row[column.Key].ToString());
                        }
                    }
                    dataSet.Columns.Remove(column.Key);
                }
            }

            return ReorderDataSet(dataSet);
        }

        //private void SelectiveCutpoints(DataTable dataSet)
        //{
        //    var dataEnumer = dataSet.AsEnumerable();
        //    foreach (var cutPoint in CutPoints)
        //    {
        //        dataEnumer.Where(t=>t.Field<float>(cutPoint.Key)>=)
        //    }
        //}

        private DataTable ReorderDataSet(DataTable dataSet)
        {
            var numeric = ColumnTypes.Where(t => t.Value == ColumnType.NUMERIC);
            int ordinal = 0;
            if (numeric.Count()>0)
            {
                foreach (var column in numeric)
                {
                    //ordinal += OriginalDataSet.Columns[column.Key].Ordinal + ordinal - 1 < 0 ?0 : OriginalDataSet.Columns[column.Key].Ordinal;
                    foreach (var cutpoint in CutPoints[column.Key])
                    {
                        dataSet.Columns[string.Concat(column.Key, "|", cutpoint)].SetOrdinal(ordinal);
                        ordinal++;
                    }
                }
            }

            //foreach (var column in ColumnTypes)
            //{
            //    if (column.Value == ColumnType.NUMERIC)
            //    {
                    
            //        foreach (var cutpoint in CutPoints[column.Key])
            //        {
            //            dataSet.Columns[string.Concat(column.Key,"|", cutpoint)].SetOrdinal(ordinal);
            //            ordinal++;
            //        }
            //    }
            //}
            return dataSet;
        }

        private void FindNumericOrSymbolic(DataTable dataSet)
        {
            var testData = dataSet.AsEnumerable();
            foreach (DataColumn column in dataSet.Columns)
            {
                bool isNumeric = true;
                var temp = testData.Select(t => t.Field<string>(column)).ToList();
                if (column.Ordinal == dataSet.Columns.Count - 2)
                    continue;
                if (column.ColumnName != "ID")
                {
                    foreach (var item in temp)
                    {
                        if (item == "?" || item == "*")
                            continue;
                        if (!float.TryParse(item, out float result))
                        {
                            isNumeric = false;
                            ColumnTypes.Add(column.ColumnName, ColumnType.SYMBOLIC);
                            break;
                        }

                    }
                    if (isNumeric)
                        ColumnTypes.Add(column.ColumnName, ColumnType.NUMERIC);
                }
            }
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
