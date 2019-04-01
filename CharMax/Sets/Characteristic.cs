using CharMax.Models;
using CharMax.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CharMax.Sets
{
    class Characteristic
    {
        public Dictionary<int, List<int>> CharacteristicSets { get; set; } = new Dictionary<int, List<int>>();

        public void FindCharacteristicSets(DataTable dataSet, AttributeValuePairs attributeValuePairs)
        {
            foreach (DataRow row in dataSet.Rows)
            {
                List<List<int>> characteristicList = new List<List<int>>();
                foreach (DataColumn col in dataSet.Columns)
                {
                    if (col.Ordinal < dataSet.Columns.Count - 2)
                    {
                        var value = row[col].ToString();

                        if (value == "*" || value == "?")
                            continue;
                        else
                        {
                            var block = attributeValuePairs.Pairs.Where(pair => pair.AttributeValue.Attribute == col.ColumnName 
                                && pair.AttributeValue.Value == value).Select(blocks => blocks.Blocks).FirstOrDefault();
                            characteristicList.Add(block);
                        }
                    }
                }
                var set = characteristicList.IntersectAll().ToList();

                CharacteristicSets.Add(int.Parse(row["ID"].ToString()), set);
            }
        }
    }
}
