﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Windows.Data.Json;

namespace Tipperesultater.Data
{
    public class JokerData : ResultatData
    {
      
        public JokerData(JsonObject jsonObjectLotto, string spillnavn) : base(spillnavn)
       {
           System.Diagnostics.Debug.WriteLine("joker");

           var a = jsonObjectLotto["drawDate"].GetString();
           DateTime trekningspunkt = DateTime.ParseExact(a, "yyyy,MM,dd,HH,mm,ss", CultureInfo.CurrentCulture);
           string trekningspunktAsString = trekningspunkt.ToString("dddd d. MMMM", CultureInfo.CurrentCulture);

           JsonArray vinnertallArray = jsonObjectLotto["digits"].GetArray();
           string vinnertallStr = String.Join(", ", vinnertallArray.Select(x => x.GetNumber()).ToList());
           string spillerkortvinner = jsonObjectLotto["winnerNr"].GetNumber().ToString("### ### ###");
           this.AntallVinnere = jsonObjectLotto["nrOfWinners"].GetNumber().ToString("### ### ###");
           JsonObject personalia = jsonObjectLotto["winnerPersonalia"].GetObject();
           String genderKode = personalia["gender"].GetString();
           var gender = genderKode.Equals("K") ?  (Utils.isEnglish() ? "female" : "kvinne") : (Utils.isEnglish() ? "male" : "mann");
           spillerkortvinner = spillerkortvinner + " (" + gender + " " + personalia["age"].GetNumber() + ", " + personalia["borough"].GetString() + ")";

           JsonArray premier = jsonObjectLotto["prizeTable"].GetArray();
           JsonArray premierTitles = jsonObjectLotto["prizeCaptionTable"].GetArray();
           string desc = String.Join("\r\n", premierTitles.Select(x => x.GetString().Replace("1.premie", "1. premie")).ToList());
           string prem = String.Join("\r\n", premier.Select(x =>
                           !Regex.IsMatch(x.GetString(), @"^\d+$") ? x.GetString() :
                           int.Parse(x.GetString()).ToString("### ### ### kr")
                        ).ToList());

           this.Vinnertall = vinnertallStr;
           this.Spillerkortnummer = spillerkortvinner;
           this.Trekningsdato = trekningspunktAsString;
           this.Premienavn = desc;
           this.Premietall = prem;
   
       }

        public string Vinnertall { get; protected set; }
        public string Spillerkortnummer { get; protected set; }
        public string Trekningsdato { get; protected set; }
        public string Premienavn { get; protected set; }
        public string Premietall { get; protected set; }
        public string AntallVinnere { get; protected set; }
    }
}
