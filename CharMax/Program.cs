using CharMax.Helper;
using CharMax.Models;
using CharMax.Sets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CharMax
{
    class Program
    {
        static void Main(string[] args)
        {
            var dataTable = FileOperation.ReadDataFile(@"C:\Users\kumar\OneDrive\Documents\Projects\CharMax\CharMax\Datasets\test.d");
            AttributeValuePairs attributeValuePairs = new AttributeValuePairs();
            attributeValuePairs.Pairs = AttributeValuePairs.FindAttributeValuePairs(dataTable);

            Characteristic characteristic = new Characteristic();

            characteristic.FindCharacteristicSets(dataTable, attributeValuePairs);

            MaximalConsistent maximalConsistent = new MaximalConsistent();

            maximalConsistent.SetMaximalConsistents(characteristic);
        }
    }
}
