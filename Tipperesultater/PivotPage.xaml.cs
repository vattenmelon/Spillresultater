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
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private async void PivotSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var statusBar = StatusBar.GetForCurrentView();
            if (isEnglish())
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

        private bool isEnglish()
        {
            return !CultureInfo.CurrentCulture.TwoLetterISOLanguageName.Equals("nb");
        }

        private async System.Threading.Tasks.Task LoadData(Boolean forceRefresh)
        {
            var sampleDataGroup = await WebDataSource.GetGroupAsync(Games[pivot.SelectedIndex].Name, forceRefresh);
            this.DefaultViewModel[Games[pivot.SelectedIndex].GroupName] = sampleDataGroup;
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
                FotballtippingLiveResultsLabel.Visibility = Visibility.Collapsed;
                FotballtippingLiveResultatValues.Visibility = Visibility.Collapsed;
                FotballtippingLiveResultatStatusValues.Visibility = Visibility.Collapsed;
                FotballtippingAntallVinnereValues.Visibility = Visibility.Collapsed;

                FotballtippingSonLiveResultsLabel.Visibility = Visibility.Collapsed;
                FotballtippingSonLiveResultatValues.Visibility = Visibility.Collapsed;
                FotballtippingSonLiveResultatStatusValues.Visibility = Visibility.Collapsed;
                FotballtippingSonAntallVinnereValues.Visibility = Visibility.Collapsed;

                FotballtippingMidtLiveResultsLabel.Visibility = Visibility.Collapsed;
                FotballtippingMidtLiveResultatValues.Visibility = Visibility.Collapsed;
                FotballtippingMidtLiveResultatStatusValues.Visibility = Visibility.Collapsed;
                FotballtippingMidtAntallVinnereValues.Visibility = Visibility.Collapsed;

                LottoAntallVinnereValues.Visibility = Visibility.Collapsed;
                VikingLottoAntallVinnereValues.Visibility = Visibility.Collapsed;

            }

            if (CurrentViewState == "Landscape")
            {
                //To Do UI for landscape
                FotballtippingLiveResultsLabel.Visibility = Visibility.Visible;
                FotballtippingLiveResultatValues.Visibility = Visibility.Visible;
                FotballtippingLiveResultatStatusValues.Visibility = Visibility.Visible;
                FotballtippingAntallVinnereValues.Visibility = Visibility.Visible;

                FotballtippingSonLiveResultsLabel.Visibility = Visibility.Visible;
                FotballtippingSonLiveResultatValues.Visibility = Visibility.Visible;
                FotballtippingSonLiveResultatStatusValues.Visibility = Visibility.Visible;
                FotballtippingSonAntallVinnereValues.Visibility = Visibility.Visible;

                FotballtippingMidtLiveResultsLabel.Visibility = Visibility.Visible;
                FotballtippingMidtLiveResultatValues.Visibility = Visibility.Visible;
                FotballtippingMidtLiveResultatStatusValues.Visibility = Visibility.Visible;
                FotballtippingMidtAntallVinnereValues.Visibility = Visibility.Visible;

                LottoAntallVinnereValues.Visibility = Visibility.Visible;
                VikingLottoAntallVinnereValues.Visibility = Visibility.Visible;

                
            }  
        }


    }
}
