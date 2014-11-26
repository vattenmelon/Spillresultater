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
    /// Generic item data model.
    /// </summary>
    public class SampleDataItem
    {
        public SampleDataItem(String uniqueId, String title, String subtitle, String imagePath, String description, String content)
        {
            this.UniqueId = uniqueId;
            this.Title = title;
            this.Subtitle = subtitle;
            this.Description = description;
            this.ImagePath = imagePath;
            this.Content = content;
        }

        public string UniqueId { get; private set; }
        public string Title { get; private set; }
        public string Subtitle { get; private set; }
        public string Description { get; private set; }
        public string ImagePath { get; private set; }
        public string Content { get; private set; }

        public override string ToString()
        {
            return this.Title;
        }
    }

    /// <summary>
    /// Generic group data model.
    /// </summary>
    public class SampleDataGroup
    {
        public SampleDataGroup(String uniqueId, String title, String subtitle, String imagePath, String description, String premier)
        {
            this.UniqueId = uniqueId;
            this.Title = title;
            this.Subtitle = subtitle;
            this.Description = description;
            this.ImagePath = imagePath;
            this.Premier = premier;
            this.Items = new ObservableCollection<SampleDataItem>();
        }

        public string UniqueId { get; private set; }
        public string Title { get; private set; }
        public string Subtitle { get; private set; }
        public string Description { get; private set; }
        public string ImagePath { get; private set; }
        public string Premier { get; private set; }
        public ObservableCollection<SampleDataItem> Items { get; private set; }

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
        /*
        public static async Task<IEnumerable<SampleDataGroup>> GetGroupsAsync()
        {
            await _sampleDataSource.GetSampleDataAsync();

            return _sampleDataSource.Groups;
        }
        */
        public static async Task<SampleDataGroup> GetGroupAsync(string uniqueId)
        {
            await _sampleDataSource.GetSampleDataAsync();
            // Simple linear search is acceptable for small data sets
            var matches = _sampleDataSource.Groups.Where((group) => group.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }
        
        public static async Task<SampleDataItem> GetItemAsync(string uniqueId)
        {
            await _sampleDataSource.GetSampleDataAsync();
            // Simple linear search is acceptable for small data sets
            var matches = _sampleDataSource.Groups.SelectMany(group => group.Items).Where((item) => item.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }
        
        private async Task GetSampleDataAsync()
        {
            if (this._groups.Count != 0)
                return;
            /*
            Uri dataUri = new Uri("ms-appx:///DataModel/SampleData.json");

            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(dataUri);
            string jsonText = await FileIO.ReadTextAsync(file);
            JsonObject jsonObject = JsonObject.Parse(jsonText);
            JsonArray jsonArray = jsonObject["Groups"].GetArray();
            */

            SampleDataGroup sd = await lagLottoGruppe("https://www.norsk-tipping.no/api-lotto/getResultInfo.json", "Group-1");
            this.Groups.Add(sd);
            SampleDataGroup sd2 = await lagLottoGruppe("https://www.norsk-tipping.no/api-vikinglotto/getResultInfo.json", "Group-2");
            this.Groups.Add(sd2);

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
            int drawId = (int)jsonObjectLotto["drawID"].GetNumber();
            JsonArray vinnertallArray = jsonObjectLotto["mainTable"].GetArray();
            var vinnertallStr = String.Join(", ", vinnertallArray.Select(x => x.GetNumber()).ToList());
            JsonArray tilleggstallArray = jsonObjectLotto["addTable"].GetArray();
            var tilleggstallStr = String.Join(", ", tilleggstallArray.Select(x => x.GetNumber()).ToList());
            var a = jsonObjectLotto["drawDate"].GetString();
            DateTime trekningspunkt = DateTime.ParseExact(a, "yyyy,MM,dd,HH,mm,ss", CultureInfo.InvariantCulture);
            string trekningspunktAsString = trekningspunkt.ToString("dddd dd. MMMM");
            JsonArray premier = jsonObjectLotto["prizeTable"].GetArray();
            JsonArray premierTitles = jsonObjectLotto["prizeCaptionTable"].GetArray();
            string desc = String.Join("\r\n", premierTitles.Select(x => x.GetString()).ToList());
            string prem = String.Join("\r\n", premier.Select(x =>
                x.GetString().Equals("Jackpot!") ? "Jackpot!" : 
                int.Parse(x.GetString()).ToString("### ### ### kr")
             ).ToList());


            SampleDataGroup sd = new SampleDataGroup(gruppenavn, vinnertallStr, tilleggstallStr, trekningspunktAsString, desc, prem);
            return sd;
        }
    }
}