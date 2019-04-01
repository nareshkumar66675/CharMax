using CharMax.Helper;
using CharMax.Models;
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

            var attValPrs = AttributeValuePairs.FindAttributeValuePairs(dataTable);

        }
    }
}
