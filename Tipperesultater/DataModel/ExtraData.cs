using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Windows.Data.Json;

namespace Tipperesultater.Data
{
    public class ExtraData : ResultatData
    {

       public ExtraData(JsonObject jsonObjectLotto, string spillnavn) : base(spillnavn)
      { 
           var a = jsonObjectLotto["drawDate"].GetString();
           DateTime trekningspunkt = DateTime.ParseExact(a, "yyyy,MM,dd,HH,mm,ss", CultureInfo.CurrentCulture);
           string trekningspunktAsString = trekningspunkt.ToString("dddd d. MMMM", CultureInfo.CurrentCulture);
           string tmpPremieTall = "";
           tmpPremieTall += int.Parse(jsonObjectLotto["prizeFirstBoard"].GetString()).ToString("### ### ### kr") + "\r\n";
           tmpPremieTall += int.Parse(jsonObjectLotto["prizeFirstFrame"].GetString()).ToString("### ### ### kr") + "\r\n";
           tmpPremieTall += int.Parse(jsonObjectLotto["prizeFirstImage"].GetString()).ToString("### ### ### kr") + "\r\n";
           tmpPremieTall += int.Parse(jsonObjectLotto["prizeBoard"].GetString()).ToString("### ### ### kr") + "\r\n";
           tmpPremieTall += int.Parse(jsonObjectLotto["prizeFrame"].GetString()).ToString("### ### ### kr") + "\r\n";
           tmpPremieTall += int.Parse(jsonObjectLotto["prizeImage"].GetString()).ToString("### ### ### kr") + "\r\n";
           tmpPremieTall += int.Parse(jsonObjectLotto["prizeExtraCandidate"].GetString()).ToString("### ### ### kr") + "\r\n";
           tmpPremieTall += int.Parse(jsonObjectLotto["extraChancePrize"].GetString()).ToString("### ### ### kr");

           string tmpPremieNavn = Utils.isEnglish() ? "First board\r\n" : "Første brett\r\n";
           tmpPremieNavn += Utils.isEnglish() ? "First frame\r\n" : "Første ramme\r\n";
           tmpPremieNavn += Utils.isEnglish() ? "First image\r\n" : "Første bilde\r\n";
           tmpPremieNavn += Utils.isEnglish() ? "Board\r\n" : "Brett\r\n";
           tmpPremieNavn += Utils.isEnglish() ? "Frame\r\n" : "Ramme\r\n";
           tmpPremieNavn += Utils.isEnglish() ? "Image\r\n" : "Bilde\r\n";
           tmpPremieNavn += Utils.isEnglish() ? "Extra candidate\r\n" : "Extrakandidaten\r\n";
           tmpPremieNavn += Utils.isEnglish() ? "Extra chanse" : "Extrasjansen";

           JsonArray winnerArray = jsonObjectLotto["winnerList"].GetArray();
           StringBuilder ekstraSjansenVinnere = new StringBuilder();
           
           foreach (JsonValue andreValue in winnerArray)
           {
               JsonArray array2 = andreValue.GetArray();
               ekstraSjansenVinnere.Append(String.Format("{0} {1}, {2} {3}\r\n", array2[2].GetString().Replace("'", ""), array2[4].GetString().Replace("'", ""), array2[5].GetString().Replace("'", ""), array2[6].GetString().Replace("'", "")));
           }


           JsonArray numberBuffer = jsonObjectLotto["numberBuffer"].GetArray();
           StringBuilder numbers = new StringBuilder();
           StringBuilder ekstraNumbers = new StringBuilder();
           Boolean leggTilEkstranummer = false;
           foreach (JsonValue value in numberBuffer)
           {
              if (value.ValueType == JsonValueType.Number)
              {
                  if (leggTilEkstranummer)
                  {
                      ekstraNumbers.Append(String.Format("{0}, ", value.GetNumber()));
                  }
                  else
                  {
                      numbers.Append(String.Format("{0,2}, ", value.GetNumber()));
                  }
              }
              else
              {
                  String bokstav = value.GetString();
                  String valueToAppend = "";
                  if ("B".Equals(bokstav))
                  {
                      if (Utils.isEnglish())
                      {
                          valueToAppend = "First image";
                      }
                      else
                      {
                          valueToAppend = "Første bilde";
                      }
                      
                  }
                  else if ("R".Equals(bokstav))
                  {
                      if (Utils.isEnglish())
                      {
                          valueToAppend = "First frame";
                      }
                      else
                      {
                          valueToAppend = "Første ramme";
                      }
                  }
                  else if ("F".Equals(bokstav))
                  {
                      if (Utils.isEnglish())
                      {
                          valueToAppend = "First board";
                      }
                      else
                      {
                          valueToAppend = "Første brett";
                      }
                  }
                  else if ("N".Equals(bokstav))
                  {
                      leggTilEkstranummer = true;
                      continue;
                  }
                  numbers.Append(String.Format("{0}, ", valueToAppend));
              }
              
           }

           this.Trekningsdato = trekningspunktAsString;
           this.Premienavn = tmpPremieNavn;
           this.Premietall = tmpPremieTall;
           this.VinnereEkstrasjansen = ekstraSjansenVinnere.ToString();
           this.Vinnertall = numbers.ToString().TrimEnd(new[]{',', ' '});
           this.Tilleggstall = ekstraNumbers.ToString().TrimEnd(new[]{',', ' '});
       }

       public string Vinnertall { get; protected set; }
       public string Tilleggstall { get; protected set; }
       public string Trekningsdato { get; protected set; }
       public string Premienavn { get; protected set; }
       public string Premietall { get; protected set; }
       public string VinnereEkstrasjansen { get; protected set; }

    }
}


