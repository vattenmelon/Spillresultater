using Tipperesultater.Common;
using Tipperesultater.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.ViewManagement;
using Windows.ApplicationModel.Activation;
using Windows.Media.SpeechSynthesis;

// The Pivot Application template is documented at http://go.microsoft.com/fwlink/?LinkID=391641

namespace Tipperesultater
{
    public sealed partial class PivotPage : Page
    {
        private const string FirstGroupName = "FirstGroup";
        private const string SecondGroupName = "SecondGroup";
        private const string ThirdGroupName = "ThirdGroup";
        private const string FourthGroupName = "FourthGroup";
        private const string FifthGroupName = "FifthGroup";
        private const string SixthGroupName = "SixthGroup";
        private const string SeventhGroupName = "SeventhGroup";
        private const string EightGroupName = "EightGroup";

        public static Dictionary<int, GroupGameName> Games = new Dictionary<int, GroupGameName>()
        {
                { 0, new GroupGameName("lotto", FirstGroupName)},
                { 1, new GroupGameName("vikinglotto", SecondGroupName)},
                { 2, new GroupGameName("joker", ThirdGroupName)},
                { 3, new GroupGameName("eurojackpot", FourthGroupName)},
                { 4, new GroupGameName("fotballtipping", FifthGroupName)}, 
                { 5, new GroupGameName("fotballtippingSon", SixthGroupName)},
                { 6, new GroupGameName("fotballtippingMidt", SeventhGroupName)},
                { 7, new GroupGameName("superlotto", EightGroupName)}
        };

        public struct GroupGameName
        {
            public string Name;
            public string GroupName;
  

            public GroupGameName(string Name, string GroupName)
            {
                this.Name = Name;
                this.GroupName = GroupName;
            }
        }

        private readonly NavigationHelper navigationHelper;
        private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();
        private readonly ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView("Resources"); //faar ikke dette til å virke..

        public PivotPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Gets the view model for this <see cref="Page"/>.
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// Populates the page with content passed during navigation. Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>.
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session. The state will be null the first time a page is visited.</param>
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache. Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/>.</param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            // TODO: Save the unique state of the page here.
        }

        /// <summary>
        /// Adds an item to the list when the app bar button is clicked.
        /// </summary>
        private async void AddAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("AddAppBarButton_Click");
            var statusBar = StatusBar.GetForCurrentView();
            await statusBar.ProgressIndicator.ShowAsync();
            await LoadData(true);
            await statusBar.ProgressIndicator.HideAsync();
        }

        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Provides data for navigation methods and event
        /// handlers that cannot cancel the navigation request.</param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
            System.Diagnostics.Debug.WriteLine("------------------>" + e);
            System.Diagnostics.Debug.WriteLine(e.Parameter);
            if (e.Parameter.Equals("ShowLottoResults"))
            {
                pivot.SelectedIndex = 0;
            }
            else if (e.Parameter.Equals("ShowVikingLottoResults"))
            {
                pivot.SelectedIndex = 1;
            }
            else if (e.Parameter.Equals("ShowJokerResults"))
            {
                pivot.SelectedIndex = 2;
            }
            else if (e.Parameter.Equals("ShowEuroJackpotResults"))
            {
                pivot.SelectedIndex = 3;
            }
            else if (e.Parameter.Equals("ShowSuperLottoResults"))
            {
                pivot.SelectedIndex = 7;
            }
            else if (e.Parameter.Equals("TellLottoResults"))
            {
                await SelectedAndTellLotto(0, "lotto");
            }
            else if (e.Parameter.Equals("TellVikingLottoResults"))
            {
                await SelectedAndTellLotto(1, "vikinglotto");
            }
            else if (e.Parameter.Equals("TellJokerResults"))
            {
                await SelectedAndTellJoker(2, "joker");
            }
            
        }

        private async System.Threading.Tasks.Task SelectedAndTellLotto(int selectedIndex, string gruppenavn)
        {
            pivot.SelectedIndex = selectedIndex;
            {
                using (var speech = new SpeechSynthesizer())
                {
                    LottoData sampleDataGroup = (LottoData)await WebDataSource.GetGroupAsync(gruppenavn, true);

                    var voiceStream = await speech.SynthesizeTextToStreamAsync("Winning numbers are " + sampleDataGroup.Vinnertall + ". Additional numbers are " + sampleDataGroup.Tilleggstall);
                    player.SetSource(voiceStream, voiceStream.ContentType);
                    player.Play();
                }
            }
        }

        private async System.Threading.Tasks.Task SelectedAndTellJoker(int selectedIndex, string gruppenavn)
        {
            pivot.SelectedIndex = selectedIndex;
            {
                using (var speech = new SpeechSynthesizer())
                {
                    JokerData sampleDataGroup = (JokerData)await WebDataSource.GetGroupAsync(gruppenavn, true);

                    var voiceStream = await speech.SynthesizeTextToStreamAsync("Winning numbers are " + sampleDataGroup.Vinnertall + ". Winning player card numbers is " + sampleDataGroup.Spillerkortnummer);
                    player.SetSource(voiceStream, voiceStream.ContentType);
                    player.Play();
                }
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private async void PivotSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var statusBar = StatusBar.GetForCurrentView();
            if (Utils.isEnglish())
            {
                statusBar.ProgressIndicator.Text = "Retreiving results";
            }
            else
            {
                statusBar.ProgressIndicator.Text = "Henter resultater";
            }
            
            await statusBar.ProgressIndicator.ShowAsync();

            await LoadData(false);

            await statusBar.ProgressIndicator.HideAsync();

            
        }


        private async System.Threading.Tasks.Task LoadData(Boolean forceRefresh)
        {
            int selectedIndex = pivot.SelectedIndex;
            var sampleDataGroup = await WebDataSource.GetGroupAsync(Games[selectedIndex].Name, forceRefresh);
            this.DefaultViewModel[Games[selectedIndex].GroupName] = sampleDataGroup;
        }


        private async void Pivot_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Pivot Doubletapped");
            var statusBar = StatusBar.GetForCurrentView();
            await statusBar.ProgressIndicator.ShowAsync();
            
            await LoadData(true);
            await statusBar.ProgressIndicator.HideAsync();
        }


        private new void SizeChanged(object sender, SizeChangedEventArgs e)
        {
            string CurrentViewState = ApplicationView.GetForCurrentView().Orientation.ToString();
            
            if (CurrentViewState == "Portrait")
            {
                LottoStackPanelVertical.Visibility = Visibility.Visible;
                LottoStackPanelHorizontal.Visibility = Visibility.Collapsed;

                VikingLottoStackPanelVertical.Visibility = Visibility.Visible;
                VikingLottoStackPanelHorizontal.Visibility = Visibility.Collapsed;

                JokerStackPanelVertical.Visibility = Visibility.Visible;
                JokerStackPanelHorizontal.Visibility = Visibility.Collapsed;

                EuroStackPanelVertical.Visibility = Visibility.Visible;
                EuroStackPanelHorizontal.Visibility = Visibility.Collapsed;

                SuperLottoStackPanelVertical.Visibility = Visibility.Visible;
                SuperLottoStackPanelHorizontal.Visibility = Visibility.Collapsed;

            }

            if (CurrentViewState == "Landscape")
            {
                //To Do UI for landscape
                LottoStackPanelVertical.Visibility = Visibility.Collapsed;
                LottoStackPanelHorizontal.Visibility = Visibility.Visible;

                VikingLottoStackPanelVertical.Visibility = Visibility.Collapsed;
                VikingLottoStackPanelHorizontal.Visibility = Visibility.Visible;

                JokerStackPanelVertical.Visibility = Visibility.Collapsed;
                JokerStackPanelHorizontal.Visibility = Visibility.Visible;

                EuroStackPanelVertical.Visibility = Visibility.Collapsed;
                EuroStackPanelHorizontal.Visibility = Visibility.Visible;

                SuperLottoStackPanelVertical.Visibility = Visibility.Collapsed;
                SuperLottoStackPanelHorizontal.Visibility = Visibility.Visible;

                
            }  
        }

       



    }
}
