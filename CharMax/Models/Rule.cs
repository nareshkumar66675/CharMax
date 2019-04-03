using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CharMax.Models
{
    class Rule
    {
        public KeyValuePair<string, List<int>> Decision { get; set; } = new KeyValuePair<string, List<int>>();

        public List<AttributeValuePair> Rules { get; set; } = new List<AttributeValuePair>();
    }
}
