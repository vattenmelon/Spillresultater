using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Windows.Data.Json;

namespace Tipperesultater.Data
{
    class JokerData : ResultatData
    {
        public JokerData(JsonObject jsonObjectLotto) : base(jsonObjectLotto)
       {
           System.Diagnostics.Debug.WriteLine("joker");

           var a = jsonObjectLotto["drawDate"].GetString();
           DateTime trekningspunkt = DateTime.ParseExact(a, "yyyy,MM,dd,HH,mm,ss", CultureInfo.CurrentCulture);
           string trekningspunktAsString = trekningspunkt.ToString("dddd d. MMMM", CultureInfo.CurrentCulture);

           JsonArray vinnertallArray = jsonObjectLotto["digits"].GetArray();
           string vinnertallStr = String.Join(", ", vinnertallArray.Select(x => x.GetNumber()).ToList());
           string tilleggstallStr = jsonObjectLotto["winnerNr"].GetNumber().ToString("### ### ###");
           JsonObject personalia = jsonObjectLotto["winnerPersonalia"].GetObject();
           String genderKode = personalia["gender"].GetString();
           var gender = genderKode.Equals("K") ? "kvinne" : "mann";
           tilleggstallStr = tilleggstallStr + " (" + gender + " " + personalia["age"].GetNumber() + ", " + personalia["borough"].GetString() + ")";

           JsonArray premier = jsonObjectLotto["prizeTable"].GetArray();
           JsonArray premierTitles = jsonObjectLotto["prizeCaptionTable"].GetArray();
           string desc = String.Join("\r\n", premierTitles.Select(x => x.GetString()).ToList());
           string prem = String.Join("\r\n", premier.Select(x =>
                           !Regex.IsMatch(x.GetString(), @"^\d+$") ? x.GetString() :
                           int.Parse(x.GetString()).ToString("### ### ### kr")
                        ).ToList());

           this.Spillnavn = "2";
           this.Vinnertall = vinnertallStr;
           this.Tilleggstall = tilleggstallStr;
           this.Trekningsdato = trekningspunktAsString;
           this.Premienavn = desc;
           this.Premietall = prem;
   
       }
    }
}
