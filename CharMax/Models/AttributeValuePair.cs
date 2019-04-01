using System;
using System.Collections.Generic;
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
        public AttributeValue AttributeValue { get; set; }

        public List<int> Blocks { get; set; } = new List<int>();
    }

    class AttributeValuePairs
    {
        public List<AttributeValuePair> Pairs { get; set; } = new List<AttributeValuePair>();
    }
}
