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

        public Characteristic(Data data)
        {
            CharacteristicSets = FindCharacteristicSets(data);
        }

        private Dictionary<int, List<int>> FindCharacteristicSets(Data data)
        {
            Dictionary<int, List<int>> tempCharSet = new Dictionary<int, List<int>>();
            foreach (DataRow row in data.DataSet.Rows)
            {
                List<List<int>> characteristicList = new List<List<int>>();
                foreach (DataColumn col in data.DataSet.Columns)
                {
                    if (col.Ordinal < data.DataSet.Columns.Count - 2)
                    {
                        var value = row[col].ToString();

                        if (value == "*" || value == "?")
                            continue;
                        else
                        {
                            var block = data.AttributeValuePairs.Where(pair => pair.AttributeValue.Attribute == col.ColumnName 
                                && pair.AttributeValue.Value == value).Select(blocks => blocks.Blocks).First();
                            characteristicList.Add(block);
                        }
                    }
                }
                var set = characteristicList.IntersectAll().ToList();

                tempCharSet.Add(int.Parse(row["ID"].ToString()), set);
            }

            return tempCharSet;
        }
    }
}
