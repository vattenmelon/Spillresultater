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
           if (spillnavn.Equals("vikinglotto"))
           {
               JsonArray lykketallArray = jsonObjectLotto["luckyTable"].GetArray();
               this.Lykketall = String.Join(", ", lykketallArray.Select(x => x.GetNumber()).ToList());
           }

           JsonArray premier = jsonObjectLotto["prizeTable"].GetArray();
           JsonArray premierTitles = jsonObjectLotto["prizeCaptionTable"].GetArray();
           string desc = String.Join("\r\n", premierTitles.Select(x => x.GetString()).ToList());
           string prem = String.Join("\r\n", premier.Select(x =>
                           !Regex.IsMatch(x.GetString(), @"^\d+$") ? x.GetString() :
                           int.Parse(x.GetString()).ToString("### ### ### kr")
                        ).ToList());

           JsonArray antallVinnereArray = jsonObjectLotto["winnerCountTable"].GetArray();
           this.AntallVinnere = String.Join("\r\n", antallVinnereArray.Select(x => formatVinnere(x)).ToList());

           if (Spillnavn.Equals("lotto"))
           {
               try
               {
                   JsonObject superLottoNextDrawObject = jsonObjectLotto["superlottoNextDraw"].GetObject();
                   DateTime superLottoNeste = DateTime.ParseExact(superLottoNextDrawObject["drawDate"].GetString(), "yyyy,MM,dd,HH,mm,ss", CultureInfo.CurrentCulture);
                   this.NesteSuperLottoTrekning = superLottoNeste.ToString("dddd d. MMMM", CultureInfo.CurrentCulture);
                   this.AntallTrekningerTilNesteSuperLotto = superLottoNextDrawObject["numberOfDrawsUntil"].GetNumber().ToString();
               }
               catch (Exception e)
               {
                   System.Diagnostics.Debug.WriteLine("kunne ikke parse superlotto nestetrekning " + e.Message);
               }
           }
           
           this.Vinnertall = vinnertallStr;
           this.Tilleggstall = tilleggstallStr;
           this.Trekningsdato = trekningspunktAsString;
           this.Premienavn = desc;
           this.Premietall = prem;
       }

       private static string formatVinnere(IJsonValue x)
       {
           if (x.GetNumber() == 0d)
           {
               return Utils.isEnglish() ? "no winners" : "ingen vinnere";
           }
           else if (x.GetNumber() == 1d)
           {
               return Utils.isEnglish() ? "1 winner" : "1 vinner";
           }
           else
           {
               return String.Format("{0} {1}", x.GetNumber().ToString("### ###"), Utils.isEnglish() ? "winners" : "vinnere");
           }

       }


       public string Vinnertall { get; protected set; }
       public string Tilleggstall { get; protected set; }
       public string Trekningsdato { get; protected set; }
       public string Premienavn { get; protected set; }
       public string Premietall { get; protected set; }
       public string AntallVinnere { get; protected set; }
       public string NesteSuperLottoTrekning { get; protected set; }
       public string AntallTrekningerTilNesteSuperLotto { get; protected set; }
       public string Lykketall { get; protected set; }
    }
}


