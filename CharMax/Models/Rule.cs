using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CharMax.Models
{
    public class Rule
    {
        public KeyValuePair<string, List<int>> Decision { get; set; } = new KeyValuePair<string, List<int>>();

        public List<AttributeValuePair> Conditions { get; set; } = new List<AttributeValuePair>();

        public List<int> Covers { get; set; } = new List<int>();
    }

    public class Rules
    {
        public Rules(string fileName, float alphaValue, List<KeyValuePair<string, Rule>> charRules, List<KeyValuePair<string, Rule>> mcbRules)
        {
            FileName = fileName;
            AlphaValue = alphaValue;
            CharacteristicRules = charRules;
            MCBRules = mcbRules;
        }

        public List<KeyValuePair<string, Rule>> CharacteristicRules { get; set; } = new List<KeyValuePair<string, Rule>>();

        public List<KeyValuePair<string, Rule>> MCBRules { get; set; } = new List<KeyValuePair<string, Rule>>();

        public string FileName { get; set; }

        public float AlphaValue { get; set; }
    }
}
