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
                { "fotballtippingMidt", "https://www.norsk-tipping.no/api-tipping/getResultInfo.json?gameDay=001"}
        };

        public ResultatData(String spillnavn, String vinnertall, String tilleggstall, String trekningsdato, String premienavn, String premietall)
        {
            this.Spillnavn = spillnavn;
            this.Vinnertall = vinnertall;
            this.Tilleggstall = tilleggstall;
            this.Trekningsdato = trekningsdato;
            this.Premienavn = premienavn;
            this.Premietall = premietall;
        }

        public ResultatData(String spillnavn, String vinnertall, String tilleggstall, String trekningsdato, String premienavn, String premietall, String tilleggspremie)
        {
            this.Spillnavn = spillnavn;
            this.Vinnertall = vinnertall;
            this.Tilleggstall = tilleggstall;
            this.Trekningsdato = trekningsdato;
            this.Premienavn = premienavn;
            this.Premietall = premietall;
            this.Tilleggspremie = tilleggspremie;
        }

        public string Spillnavn { get; private set; }
        public string Vinnertall { get; private set; }
        public string Tilleggstall { get; private set; }
        public string Trekningsdato { get; private set; }
        public string Premienavn { get; private set; }
        public string Premietall { get; private set; }
        public string Tilleggspremie { get; private set; }

        public override string ToString()
        {
            return this.Vinnertall;
        }
    }
}
