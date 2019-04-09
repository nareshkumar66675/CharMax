using CharMax.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CharMax.Models
{
    /// <summary>
    /// [(Temperature,medium)]
    /// </summary>
    public class AttributeValue
    {
        public string Attribute { get; set; }
        public string Value { get; set; }

        public override bool Equals(object obj)
        {
            var value = obj as AttributeValue;
            return value != null &&
                   Attribute == value.Attribute &&
                   Value == value.Value;
        }

        public override int GetHashCode()
        {
            var hashCode = -1299389615;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Attribute);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Value);
            return hashCode;
        }

        public static bool operator ==(AttributeValue value1, AttributeValue value2)
        {
            return EqualityComparer<AttributeValue>.Default.Equals(value1, value2);
        }

        public static bool operator !=(AttributeValue value1, AttributeValue value2)
        {
            return !(value1 == value2);
        }
    }

    /// <summary>
    /// [(Temperature,medium)] {1,2,3,4}
    /// </summary>
    public class AttributeValuePair
    {
        public AttributeValue AttributeValue { get; set; } = new AttributeValue();

        public List<int> Blocks { get; set; } = new List<int>();

        public static List<AttributeValuePair> GetAttributeValuePairs(DataTable dataSet)
        {
            var dataEnumer = dataSet.AsEnumerable();

            List<AttributeValuePair> pairs = new List<AttributeValuePair>();

            foreach (DataColumn column in dataSet.Columns)
            {
                if (column.Ordinal < dataSet.Columns.Count - 2)
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
