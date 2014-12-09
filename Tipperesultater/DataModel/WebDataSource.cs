using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Storage;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

// The data model defined by this file serves as a representative example of a strongly-typed
// model.  The property names chosen coincide with data bindings in the standard item templates.
//
// Applications may use this model as a starting point and build on it, or discard it entirely and
// replace it with something appropriate to their needs. If using this model, you might improve app 
// responsiveness by initiating the data loading task in the code behind for App.xaml when the app 
// is first launched.

namespace Tipperesultater.Data
{
  
    /// <summary>
    /// Creates a collection of groups and items with content read from a static json file.
    /// 
    /// SampleDataSource initializes with data read from a static json file included in the 
    /// project.  This provides sample data at both design-time and run-time.
    /// </summary>
    public sealed class WebDataSource
    {
        private static WebDataSource _sampleDataSource = new WebDataSource();

        private ObservableCollection<ResultatData> _groups = new ObservableCollection<ResultatData>();
        public ObservableCollection<ResultatData> Groups
        {
            get { return this._groups; }
        }

        public static async Task<ResultatData> GetGroupAsync(string uniqueId, Boolean forceRefresh)
        {
            var matches = _sampleDataSource.Groups.Where((group) => group.Spillnavn.Equals(uniqueId));
            if (matches.Count() == 1)
            {
                System.Diagnostics.Debug.WriteLine(String.Format("Found data for {0} in cache", uniqueId));
                if (forceRefresh)
                {
                    System.Diagnostics.Debug.WriteLine(String.Format("Forcing refresh for {0} in cache", uniqueId));
                    _sampleDataSource.Groups.Remove(matches.First());
                    var sd = await RetrieveData(ResultatData.spill[uniqueId], uniqueId);
                    _sampleDataSource.Groups.Add(sd);
                    return sd;
                }
                return matches.First();
            }
            var sd2 = await RetrieveData(ResultatData.spill[uniqueId], uniqueId);
            _sampleDataSource.Groups.Add(sd2);
            return sd2;
        }
                

        private static async Task<ResultatData> RetrieveData(String url, String gruppenavn)
        {
            Uri dataUri2 = new Uri(url);
            var client = new HttpClient();
            var response = await client.GetAsync(dataUri2);
            var result = await response.Content.ReadAsStringAsync();
            result = result.Replace("while(true);/* 0;", "");
            result = result.Replace("/* */", "");
            JsonObject jsonObjectLotto = JsonObject.Parse(result);
            string halvTid = "HALFTIME";
            string helTid = "FULLTIME";
            if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName.Equals("nb"))
            {
                halvTid = "HALVTID";
                helTid = "HELTID";
            }
            
            if (gruppenavn.StartsWith("fotballtipping"))
            {
                System.Diagnostics.Debug.WriteLine("fotballtipping");
                var b = jsonObjectLotto["gameDays"].GetArray();
                foreach (JsonValue obj in b)
                {
                    {
                        System.Diagnostics.Debug.WriteLine("----------------------");
                        if (obj.ValueType != JsonValueType.Null)
                        {
                            JsonObject ob1 = obj.GetObject();

                            string dd1 = ob1["drawDate"].GetString();
                            System.Diagnostics.Debug.WriteLine(dd1);

                            DateTime trekningspunkt2 = DateTime.ParseExact(dd1, "yyyy,MM,dd,HH,mm,ss", CultureInfo.CurrentCulture);
                            string trekningspunktAsString2 = trekningspunkt2.ToString("dddd d. MMMM", CultureInfo.CurrentCulture);

                            JsonArray events = ob1["events"].GetArray();
                            String kamper = "";
                            String kampstatus = "";
                            int tellert = 1;
                            int statusteller = 0;
                            foreach (JsonValue ev in events)
                            {
                                
                                JsonArray arra = ev.GetArray();
                                String lag1 = arra[1].GetString();
                                String lag2 = arra[2].GetString();
                                kamper += lag1 + " - " + lag2 + "\r\n";
                                if (tellert % 3 == 0)
                                {
                                    kamper += "\r\n";
                                }
                                tellert++;
                                String status = arra[4].GetString();
                                if (status.Equals("Slutt"))
                                {
                                    statusteller++;
                                }

                            }
                            if (statusteller == 12)
                            {
                                if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName.Equals("nb"))
                                {
                                    kampstatus = "Alle kamper er ferdig";
                                }
                                else
                                {
                                    kampstatus = "All matches are finished";
                                }
                            }
                            else
                            {
                                if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName.Equals("nb"))
                                {
                                    kampstatus = "Ikke alle kamper er ferdig";
                                }
                                else
                                {
                                    kampstatus = "Not all matches are finished";
                                }
                                
                            }


                            String halvtid = "";
                            String heltid = "";
                            String premieTekst = halvTid + "\r\n";
                            String premieVerdi = "\r\n";
                            int teller = 0;
                            JsonArray results = ob1["matchStages"].GetArray();
                            foreach (JsonValue ev in results) 
                            {

                                String resultatText = null;
                                JsonObject r2 = ev.GetObject();
                                JsonArray res2 = r2["results"].GetArray();
                                resultatText = GetFotballtippingResultater(res2);

                                if (teller == 0)
                                {
                                    halvtid += resultatText;
                                }
                                else
                                {
                                    heltid += resultatText;
                                }
                               
                                teller++;

                                res2 = r2["prizes"].GetArray();

                                IList abba = res2.Reverse().ToList();
                                
                                foreach (JsonValue j2 in abba)
                                {
                                    
                                    JsonArray j22 = j2.GetArray();
                                    String text = String.Join("", j22.Select((x, i) => i == 0 ? x.GetString().Replace("av", CultureInfo.CurrentCulture.TwoLetterISOLanguageName.Equals("nb") ? "av" : "of") : i == 1 ? "" : "\r\n"));
                                    premieTekst += text;
                                    String text2 = String.Join("", j22.Select((x, i) => i == 1 ? 
                                            x.ValueType == JsonValueType.String ? x.GetString() :
                                            x.GetNumber().ToString("### ### ### kr") : i == 0 ? "" : "\r\n"));
                                    premieVerdi += text2;
                                }
                                if (teller == 1)
                                {
                                    premieTekst += "\r\n"+helTid+"\r\n";
                                    premieVerdi += "\r\n\r\n";
                                }

                            }

                            return new ResultatData(heltid, kamper, halvtid, trekningspunktAsString2, premieTekst, premieVerdi, kampstatus);
                        }
                    }
                }
            }

           
            var a = jsonObjectLotto["drawDate"].GetString();
            DateTime trekningspunkt = DateTime.ParseExact(a, "yyyy,MM,dd,HH,mm,ss", CultureInfo.CurrentCulture);
            string trekningspunktAsString = trekningspunkt.ToString("dddd d. MMMM", CultureInfo.CurrentCulture);


            String vinnertallStr = null;
            String tilleggstallStr = null;
            if (gruppenavn.Equals("lotto") || gruppenavn.Equals("vikinglotto"))
            {
               System.Diagnostics.Debug.WriteLine("Ikke joker");
               JsonArray vinnertallArray = jsonObjectLotto["mainTable"].GetArray();
               vinnertallStr = String.Join(", ", vinnertallArray.Select(x => x.GetNumber()).ToList());
               JsonArray tilleggstallArray = jsonObjectLotto["addTable"].GetArray();
               tilleggstallStr = String.Join(", ", tilleggstallArray.Select(x => x.GetNumber()).ToList());
            }
            else if (gruppenavn.Equals("joker"))
            {
                System.Diagnostics.Debug.WriteLine("joker");
                JsonArray vinnertallArray = jsonObjectLotto["digits"].GetArray();
                vinnertallStr = String.Join(", ", vinnertallArray.Select(x => x.GetNumber()).ToList());
                tilleggstallStr = jsonObjectLotto["winnerNr"].GetNumber().ToString("### ### ###");
                JsonObject personalia = jsonObjectLotto["winnerPersonalia"].GetObject();
                String genderKode = personalia["gender"].GetString();
                var gender = genderKode.Equals("K") ? "kvinne" : "mann";
                tilleggstallStr = tilleggstallStr + " (" + gender + " " + personalia["age"].GetNumber() + ", " + personalia["borough"].GetString() + ")";
            }
            else if (gruppenavn.Equals("eurojackpot"))
            {
                System.Diagnostics.Debug.WriteLine("Eurojackpot");
                JsonArray vinnertallArray = jsonObjectLotto["mainTable"].GetArray();
                vinnertallStr = String.Join(", ", vinnertallArray.Select(x => x.GetNumber()).ToList());
                JsonArray tilleggstallArray = jsonObjectLotto["starTable"].GetArray();
                tilleggstallStr = String.Join(", ", tilleggstallArray.Select(x => x.GetNumber()).ToList());
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

                return new ResultatData(gruppenavn, vinnertallStr, tilleggstallStr, trekningspunktAsString, desce, pr, merc);
            }
            
            JsonArray premier = jsonObjectLotto["prizeTable"].GetArray();
            JsonArray premierTitles = jsonObjectLotto["prizeCaptionTable"].GetArray();
            string desc = String.Join("\r\n", premierTitles.Select(x => x.GetString()).ToList());
            string prem = String.Join("\r\n", premier.Select(x =>
                            !Regex.IsMatch(x.GetString(), @"^\d+$") ? x.GetString() :
                            int.Parse(x.GetString()).ToString("### ### ### kr")
                         ).ToList());


            return new ResultatData(gruppenavn, vinnertallStr, tilleggstallStr, trekningspunktAsString, desc, prem);
            
        }

        private static string GetFotballtippingResultater(JsonArray res2)
        {
            String text = null;
            int pteller = 1;
            
            foreach (JsonValue r22 in res2)
            {
                JsonArray j2 = r22.GetArray();
                text += String.Join("", j2.Select((x, i) => i == 0 ? x.GetString() : decode(x.GetNumber())));
                if (pteller % 3 == 0)
                {
                    text += "\r\n";
                }
                pteller++;



            }
            return text;
        }

        private static string decode(double number)
        {
            switch((int)number)
            {
                case 0:
                    return " H\r\n";
                case 1:
                    return " U\r\n";
                case 2:
                    return " B\r\n";
                default:
                    return "\r\n";
            }
        }
    }
}