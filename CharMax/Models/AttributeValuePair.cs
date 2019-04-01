using CharMax.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CharMax.Models
{
    class AttributeValue
    {
        public string Attribute { get; set; }
        public string Value { get; set; }

    }

    class AttributeValuePair
    {
        public AttributeValue AttributeValue { get; set; } = new AttributeValue();

        public List<int> Blocks { get; set; } = new List<int>();
    }

    class AttributeValuePairs
    {
        public List<AttributeValuePair> Pairs { get; set; } = new List<AttributeValuePair>();

        public static List<AttributeValuePair> FindAttributeValuePairs(DataTable dataTable)
        {
            var dataEnumer = dataTable.AsEnumerable();

            List<AttributeValuePair> pairs = new List<AttributeValuePair>();

            foreach (DataColumn column in dataTable.Columns)
            {
                if(column.Ordinal < dataTable.Columns.Count -2)
                {
                    var distinctValues = dataEnumer.FindDistinctValues<string>(column);

                    distinctValues.RemoveAll(t => t == "*" || t == "?");

                    var starValues = dataEnumer.FindRecords(column, "*");
                    
                    foreach (var value in distinctValues)
                    {
                        AttributeValuePair attributeValuePair = new AttributeValuePair();

                        attributeValuePair.AttributeValue.Attribute = column.ColumnName;
                        attributeValuePair.AttributeValue.Value = value;
                        attributeValuePair.Blocks = dataEnumer.FindRecords(column, value).Union(starValues).ToList();

                        pairs.Add(attributeValuePair);
                    }
                }

            }
            return pairs;
        }

    }
}
