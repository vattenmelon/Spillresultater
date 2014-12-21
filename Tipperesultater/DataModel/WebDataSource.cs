using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Data.Json;

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

            if (gruppenavn.StartsWith("fotballtipping"))
            {
                return new FotballTippingData(jsonObjectLotto, gruppenavn);
            }
            if (gruppenavn.Equals("lotto") || gruppenavn.Equals("vikinglotto"))
            {
                return new LottoData(jsonObjectLotto, gruppenavn);
            }
            else if (gruppenavn.Equals("joker"))
            {
                return new JokerData(jsonObjectLotto, gruppenavn);
            }
            else if (gruppenavn.Equals("eurojackpot"))
            {
                return new EuroJackpotData(jsonObjectLotto, gruppenavn);

            }
            else if (gruppenavn.Equals("superlotto"))
            {
                LottoData lottoData = (LottoData) await GetGroupAsync("lotto", false);
                return new SuperLottoData(jsonObjectLotto, gruppenavn, lottoData);
            }
            return null;

        }


    }
}