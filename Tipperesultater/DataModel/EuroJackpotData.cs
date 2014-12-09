using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Windows.Data.Json;

namespace Tipperesultater.Data
{
    class EuroJackpotData : ResultatData
    {
       public EuroJackpotData(JsonObject jsonObjectLotto) : base(jsonObjectLotto)
       {
           System.Diagnostics.Debug.WriteLine("Eurojackpot");
           var a = jsonObjectLotto["drawDate"].GetString();
           DateTime trekningspunkt = DateTime.ParseExact(a, "yyyy,MM,dd,HH,mm,ss", CultureInfo.CurrentCulture);
           string trekningspunktAsString = trekningspunkt.ToString("dddd d. MMMM", CultureInfo.CurrentCulture);

           JsonArray vinnertallArray = jsonObjectLotto["mainTable"].GetArray();
           String vinnertallStr = String.Join(", ", vinnertallArray.Select(x => x.GetNumber()).ToList());
           JsonArray tilleggstallArray = jsonObjectLotto["starTable"].GetArray();
           String tilleggstallStr = String.Join(", ", tilleggstallArray.Select(x => x.GetNumber()).ToList());
           JsonArray prizes = jsonObjectLotto["prizes"].GetArray();
           string desce = String.Join("\r\n", prizes.Select(x => x.GetArray()[0].GetString()).ToList());
           string pr = String.Join("\r\n", prizes.Select(x =>
                       !Regex.IsMatch(x.GetArray()[1].GetString(), @"^\d+$") ? x.GetArray()[1].GetString() :
                       int.Parse(x.GetArray()[1].GetString()).ToString("### ### ### kr")
               ).ToList());

           JsonArray merchandises = jsonObjectLotto["merchandise"].GetArray();
           String merc = "";
           int teller = 1;
           foreach (JsonValue obj in merchandises)
           {
               JsonObject o = obj.GetObject();
               JsonArray ja = o["winnerDetails"].GetArray();
               JsonArray ja2 = ja[0].GetArray();
               merc += teller + ". " + o["name"].GetString().ToUpper() + ":\r\n    " + ja2[0].GetString() + "\r\n    " + ja2[1].GetString() + ", " + ja2[2].GetString() + "\r\n\r\n";
               teller++;
           }

           this.Spillnavn = "1";
           this.Vinnertall = vinnertallStr;
           this.Tilleggstall = tilleggstallStr;
           this.Trekningsdato = trekningspunktAsString;
           this.Premienavn = desce;
           this.Premietall = pr;
           this.Tilleggspremie = merc;
       }
    }
}


