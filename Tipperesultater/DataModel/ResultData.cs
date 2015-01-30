using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tipperesultater.Data
{
    /// <summary>
    /// Generic group data model.
    /// </summary>
    public class ResultatData
    {

        public static Dictionary<string, string> spill = new Dictionary<string, string>()
        {
                { "lotto", "https://www.norsk-tipping.no/api-lotto/getResultInfo.json"},
                { "vikinglotto", "https://www.norsk-tipping.no/api-vikinglotto/getResultInfo.json"},
                { "joker", "https://www.norsk-tipping.no/api-joker/getResultInfo.json"},
                { "eurojackpot", "https://www.norsk-tipping.no/api-eurojackpot/getResultInfo.json"},
                { "fotballtipping", "https://www.norsk-tipping.no/api-tipping/getResultInfo.json?gameDay=100"}, //100 = lørdag, 010 = søndag, 001 = onsdag/midtuke
                { "fotballtippingSon", "https://www.norsk-tipping.no/api-tipping/getResultInfo.json?gameDay=010"},
                { "fotballtippingMidt", "https://www.norsk-tipping.no/api-tipping/getResultInfo.json?gameDay=001"},
                { "superlotto", "https://www.norsk-tipping.no/api-lotto/getSuperLottoResultInfo.json"},
                { "extra", "https://www.norsk-tipping.no/api-extra/getResultInfo.json"}
        };

        public ResultatData(string spillnavn)
        {
            
            this.Spillnavn = spillnavn;
        }
        public string Spillnavn { get; protected set; }

        public string ConvertToProperNameCase(string input)
        {

            char[] chars = toTitleCase(input).ToCharArray();

            for (int i = 0; i + 1 < chars.Length; i++)
            {
                if ((chars[i].Equals('\'')) ||
                    (chars[i].Equals('-')))
                {
                    chars[i + 1] = Char.ToUpper(chars[i + 1]);
                }
            }
            return new string(chars);
        }

        private string toTitleCase(string value)
        {
            if (value == null)
                return null;
            if (value.Length == 0)
                return value;

            StringBuilder result = new StringBuilder(value);
            result[0] = char.ToUpper(result[0]);
            for (int i = 1; i < result.Length; ++i)
            {
                if (char.IsWhiteSpace(result[i - 1]))
                    result[i] = char.ToUpper(result[i]);
                else
                    result[i] = char.ToLower(result[i]);
            }
            return result.ToString();
        }
    }
}
