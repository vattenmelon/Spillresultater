using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Net.Http;
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
    public class SampleDataGroup
    {

        public static Dictionary<string, string> spill = new Dictionary<string, string>()
        {
                { "lotto", "https://www.norsk-tipping.no/api-lotto/getResultInfo.json"},
                { "vikinglotto", "https://www.norsk-tipping.no/api-vikinglotto/getResultInfo.json"},
                { "joker", "https://www.norsk-tipping.no/api-joker/getResultInfo.json"},
                { "eurojackpot", "https://www.norsk-tipping.no/api-eurojackpot/getResultInfo.json"}
        };

        public SampleDataGroup(String uniqueId, String title, String subtitle, String imagePath, String description, String premier)
        {
            this.UniqueId = uniqueId;
            this.Title = title;
            this.Subtitle = subtitle;
            this.Description = description;
            this.ImagePath = imagePath;
            this.Premier = premier;
 
        }

        public string UniqueId { get; private set; }
        public string Title { get; private set; }
        public string Subtitle { get; private set; }
        public string Description { get; private set; }
        public string ImagePath { get; private set; }
        public string Premier { get; private set; }

        public override string ToString()
        {
            return this.Title;
        }
    }

    /// <summary>
    /// Creates a collection of groups and items with content read from a static json file.
    /// 
    /// SampleDataSource initializes with data read from a static json file included in the 
    /// project.  This provides sample data at both design-time and run-time.
    /// </summary>
    public sealed class SampleDataSource
    {
        private static SampleDataSource _sampleDataSource = new SampleDataSource();

        private ObservableCollection<SampleDataGroup> _groups = new ObservableCollection<SampleDataGroup>();
        public ObservableCollection<SampleDataGroup> Groups
        {
            get { return this._groups; }
        }

        public static async Task<SampleDataGroup> GetGroupAsync(string uniqueId, Boolean forceRefresh)
        {
            var matches = _sampleDataSource.Groups.Where((group) => group.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1)
            {
                System.Diagnostics.Debug.WriteLine(String.Format("Found data for {0} in cache", uniqueId));
                if (forceRefresh)
                {
                    System.Diagnostics.Debug.WriteLine(String.Format("Forcing refresh for {0} in cache", uniqueId));
                    _sampleDataSource.Groups.Remove(matches.First());
                    var sd = await lagLottoGruppe(SampleDataGroup.spill[uniqueId], uniqueId);
                    _sampleDataSource.Groups.Add(sd);
                    return sd;
                }
                return matches.First();
            }
            var sd2 = await lagLottoGruppe(SampleDataGroup.spill[uniqueId], uniqueId);
            _sampleDataSource.Groups.Add(sd2);
            return sd2;
        }
                

        private static async Task<SampleDataGroup> lagLottoGruppe(String url, String gruppenavn)
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
                var gender = genderKode.Equals("K") ? "Kvinne" : "Mann";
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
                String desce = "";
                System.Diagnostics.Debug.WriteLine("lol");
                foreach (IJsonValue arra in prizes)
                {
                    System.Diagnostics.Debug.WriteLine("lols");
                    System.Diagnostics.Debug.WriteLine(arra.GetType());
                    System.Diagnostics.Debug.WriteLine(arra.ValueType);
                    foreach (JsonValue valu in arra.GetArray())
                    {
                        System.Diagnostics.Debug.WriteLine("hahaha");
                        desce += valu.GetString();
                    }
                    
                }
                
                return new SampleDataGroup(gruppenavn, vinnertallStr, tilleggstallStr, trekningspunktAsString, desce, "");
            }
            
            JsonArray premier = jsonObjectLotto["prizeTable"].GetArray();
            JsonArray premierTitles = jsonObjectLotto["prizeCaptionTable"].GetArray();
            string desc = String.Join("\r\n", premierTitles.Select(x => x.GetString()).ToList());
            string prem = String.Join("\r\n", premier.Select(x =>
                x.GetString().Equals("Jackpot!") ? "Jackpot!" : 
                int.Parse(x.GetString()).ToString("### ### ### kr")
             ).ToList());


            return new SampleDataGroup(gruppenavn, vinnertallStr, tilleggstallStr, trekningspunktAsString, desc, prem);
            
        }
    }
}