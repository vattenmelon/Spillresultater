﻿using System;
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
    /// Generic group data model.
    /// </summary>
    public class ResultatData
    {

        public static Dictionary<string, string> spill = new Dictionary<string, string>()
        {
                { "lotto", "https://www.norsk-tipping.no/api-lotto/getResultInfo.json"},
                { "vikinglotto", "https://www.norsk-tipping.no/api-vikinglotto/getResultInfo.json"},
                { "joker", "https://www.norsk-tipping.no/api-joker/getResultInfo.json"},
                { "eurojackpot", "https://www.norsk-tipping.no/api-eurojackpot/getResultInfo.json"}
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
                    var sd = await lagLottoGruppe(ResultatData.spill[uniqueId], uniqueId);
                    _sampleDataSource.Groups.Add(sd);
                    return sd;
                }
                return matches.First();
            }
            var sd2 = await lagLottoGruppe(ResultatData.spill[uniqueId], uniqueId);
            _sampleDataSource.Groups.Add(sd2);
            return sd2;
        }
                

        private static async Task<ResultatData> lagLottoGruppe(String url, String gruppenavn)
        {
            Uri dataUri2 = new Uri(url);
            var client = new HttpClient();
            var response = await client.GetAsync(dataUri2);
            var result = await response.Content.ReadAsStringAsync();
            result = result.Replace("while(true);/* 0;", "");
            result = result.Replace("/* */", "");

            JsonObject jsonObjectLotto = JsonObject.Parse(result);
            var a = jsonObjectLotto["drawDate"].GetString();
            DateTime trekningspunkt = DateTime.ParseExact(a, "yyyy,MM,dd,HH,mm,ss", new CultureInfo("nb-NO"));
            string trekningspunktAsString = trekningspunkt.ToString("dddd dd. MMMM", new CultureInfo("nb-NO"));

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
    }
}