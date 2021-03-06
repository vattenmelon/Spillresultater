﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Windows.Data.Json;

namespace Tipperesultater.Data
{
    public class SuperLottoData : ResultatData
    {

       public SuperLottoData(JsonObject jsonObjectLotto, string spillnavn, LottoData lottoData) : base(spillnavn)
      {
          JsonArray arr = jsonObjectLotto["superlottoDraws"].GetArray();
          JsonObject sisteTrekning = arr[0].GetObject();
           var a = sisteTrekning["drawDate"].GetString();
           DateTime trekningspunkt = DateTime.ParseExact(a, "yyyy,MM,dd,HH,mm,ss", CultureInfo.CurrentCulture);
           string trekningspunktAsString = trekningspunkt.ToString("dddd d. MMMM", CultureInfo.CurrentCulture);

           
           JsonObject jObject = sisteTrekning["superlottoResult"].GetObject();
           this.AntallVinnere = jObject["numberOfWinners"].GetNumber().ToString();
           double prizeAsDouble = jObject["prizeAmount"].GetNumber();
           String prizeAsString = prizeAsDouble.ToString("### ### ### kr");
           this.Premie = prizeAsString;
           this.Trekningsdato = trekningspunktAsString;
           this.Vinnere = String.Join("\r\n", jObject["winnerList"].GetArray().Select(x =>
                           (x.GetArray()[0].ValueType == JsonValueType.String && !x.GetArray()[0].GetString().Equals("Andelsbank") ? int.Parse(x.GetArray()[0].GetString()).ToString("### ### ###") : x.GetArray()[0].GetString().Equals("Andelsbank") ? "Andelsbank" : "N/A") + ": " + decodeGender(x.GetArray()[1].GetString()) + ", " + UpperFirst(x.GetArray()[4].GetString()) + ", " + x.GetArray()[6].GetString()
                        ).ToList());
           if (Utils.isEnglish())
           { 
               this.NesteTrekning = String.Format("{0} (in {1} draws)", lottoData.NesteSuperLottoTrekning, lottoData.AntallTrekningerTilNesteSuperLotto);
           }
           else
           {
               this.NesteTrekning = String.Format("{0} ({1} trekninger til)", lottoData.NesteSuperLottoTrekning, lottoData.AntallTrekningerTilNesteSuperLotto);
           }
       }

       private string UpperFirst(string text)
       {
           if (text.Count() == 0)
               return "";
           return char.ToUpper(text[0]) +
               ((text.Length > 1) ? text.Substring(1).ToLower() : string.Empty);
       }

       private string decodeGender(string p)
       {
           if (p.Equals("K"))
           {
               return Utils.isEnglish() ? "Female" : "Kvinne";
           }
           else if (p.Equals("M"))
           {
               return Utils.isEnglish() ? "Male" : "Mann";
           }
           return "";
       }


       public string AntallVinnere { get; protected set; }
       public string Trekningsdato { get; protected set; }
       public string Premie { get; protected set; }
       public string Vinnere { get; protected set; }
       public string NesteTrekning { get; protected set; }
      
    }

}


