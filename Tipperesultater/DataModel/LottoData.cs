using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Windows.Data.Json;

namespace Tipperesultater.Data
{
    public class LottoData : ResultatData
    {

       public LottoData(JsonObject jsonObjectLotto, string spillnavn) : base(spillnavn)
      { 
           var a = jsonObjectLotto["drawDate"].GetString();
           DateTime trekningspunkt = DateTime.ParseExact(a, "yyyy,MM,dd,HH,mm,ss", CultureInfo.CurrentCulture);
           string trekningspunktAsString = trekningspunkt.ToString("dddd d. MMMM", CultureInfo.CurrentCulture);

      
           JsonArray vinnertallArray = jsonObjectLotto["mainTable"].GetArray();
           String vinnertallStr = String.Join(", ", vinnertallArray.Select(x => x.GetNumber()).ToList());
           JsonArray tilleggstallArray = jsonObjectLotto["addTable"].GetArray();
           String tilleggstallStr = String.Join(", ", tilleggstallArray.Select(x => x.GetNumber()).ToList());

           JsonArray premier = jsonObjectLotto["prizeTable"].GetArray();
           JsonArray premierTitles = jsonObjectLotto["prizeCaptionTable"].GetArray();
           string desc = String.Join("\r\n", premierTitles.Select(x => x.GetString()).ToList());
           string prem = String.Join("\r\n", premier.Select(x =>
                           !Regex.IsMatch(x.GetString(), @"^\d+$") ? x.GetString() :
                           int.Parse(x.GetString()).ToString("### ### ### kr")
                        ).ToList());

           this.Spillnavn = "3";
           this.Vinnertall = vinnertallStr;
           this.Tilleggstall = tilleggstallStr;
           this.Trekningsdato = trekningspunktAsString;
           this.Premienavn = desc;
           this.Premietall = prem;
       }


       public string Vinnertall { get; protected set; }
       public string Tilleggstall { get; protected set; }
       public string Trekningsdato { get; protected set; }
       public string Premienavn { get; protected set; }
       public string Premietall { get; protected set; }
    }
}


