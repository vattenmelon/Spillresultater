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

        private readonly NavigationHelper navigationHelper;
        private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();
        private readonly ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView("Resources");

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
        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("LoadState");
            // TODO: Create an appropriate data model for your problem domain to replace the sample data
           // var sampleDataGroup = await SampleDataSource.GetGroupAsync("Group-1");
           // this.DefaultViewModel[FirstGroupName] = sampleDataGroup;
          //  var sampleDataGroup2 = await SampleDataSource.GetGroupAsync("Group-2");
          //  this.DefaultViewModel[SecondGroupName] = sampleDataGroup2;
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
        private void AddAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            /*
            string groupName = this.pivot.SelectedIndex == 0 ? FirstGroupName : SecondGroupName;
            var group = this.DefaultViewModel[groupName] as SampleDataGroup;
            var nextItemId = group.Items.Count + 1;
            var newItem = new SampleDataItem(
                string.Format(CultureInfo.InvariantCulture, "Group-{0}-Item-{1}", this.pivot.SelectedIndex + 1, nextItemId),
                string.Format(CultureInfo.CurrentCulture, this.resourceLoader.GetString("NewItemTitle"), nextItemId),
                string.Empty,
                string.Empty,
                this.resourceLoader.GetString("NewItemDescription"),
                string.Empty);

            group.Items.Add(newItem);

            // Scroll the new item into view.
            var container = this.pivot.ContainerFromIndex(this.pivot.SelectedIndex) as ContentControl;
            var listView = container.ContentTemplateRoot as ListView;
            listView.ScrollIntoView(newItem, ScrollIntoViewAlignment.Leading);
             * */
        }

        /// <summary>
        /// Invoked when an item within a section is clicked.
        /// </summary>
        private void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("ItemView ItemClick");
        }

        /// <summary>
        /// Loads the content for the second pivot item when it is scrolled into view.
        /// </summary>
        private async void SecondPivot_Loaded(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Second Loaded");
        //    var sampleDataGroup = await SampleDataSource.GetGroupAsync("Group-2");
        //    this.DefaultViewModel[SecondGroupName] = sampleDataGroup;
        }

        private async void FirstPivot_Loaded(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("FirstPivot Loaded");
        //    var sampleDataGroup = await SampleDataSource.GetGroupAsync("Group-1");
        //    this.DefaultViewModel[FirstGroupName] = sampleDataGroup;
            
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

        private  void SecondPivot_GotFocus(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Second GotFocus");
            //var sampleDataGroup = await SampleDataSource.GetGroupAsync("Group-2");
            //this.DefaultViewModel[SecondGroupName] = sampleDataGroup;
        }

        private void FirstPivot_GotFocus(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("First GotFocus");
            //var sampleDataGroup = await SampleDataSource.GetGroupAsync("Group-1");
            //this.DefaultViewModel[SecondGroupName] = sampleDataGroup;
        }

        private async void FirstPivotDoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            /*
            System.Diagnostics.Debug.WriteLine("Doubletapped");
            var statusBar = StatusBar.GetForCurrentView();
            await statusBar.ProgressIndicator.ShowAsync();
            await LoadData(true);
            await statusBar.ProgressIndicator.HideAsync();
             */
        }

        private async void PivotSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var statusBar = StatusBar.GetForCurrentView();
            
            await statusBar.ProgressIndicator.ShowAsync();

            await LoadData(false);

            await statusBar.ProgressIndicator.HideAsync();

            
        }

        private async System.Threading.Tasks.Task LoadData(Boolean forceRefresh)
        {
            switch (pivot.SelectedIndex)
            {
                case 0:
                    System.Diagnostics.Debug.WriteLine("Getting data for first pivot");
                    var sampleDataGroup1 = await SampleDataSource.GetGroupAsync("lotto", forceRefresh);
                    this.DefaultViewModel[FirstGroupName] = sampleDataGroup1;
                    break;
                case 1:
                    System.Diagnostics.Debug.WriteLine("Getting data for second pivot");
                    var sampleDataGroup2 = await SampleDataSource.GetGroupAsync("vikinglotto", forceRefresh);
                    this.DefaultViewModel[SecondGroupName] = sampleDataGroup2;
                    break;
                case 2:
                    System.Diagnostics.Debug.WriteLine("Getting data for second pivot");
                    var sampleDataGroup3 = await SampleDataSource.GetGroupAsync("joker", forceRefresh);
                    this.DefaultViewModel[ThirdGroupName] = sampleDataGroup3;
                    break;
                default:
                    System.Diagnostics.Debug.WriteLine("Unknown index selected");
                    break;
            }
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            /*
            System.Diagnostics.Debug.WriteLine("Mainpage Loaded");
            var progressIndicator = SystemTray.ProgressIndicator;
            if (progressIndicator != null)
            {
                return;
            }

            progressIndicator = new ProgressIndicator();

            SystemTray.SetProgressIndicator(this, progressIndicator);

            Binding binding = new Binding("IsLoading") { Source = _viewModel };
            BindingOperations.SetBinding(
                progressIndicator, ProgressIndicator.IsVisibleProperty, binding);

            binding = new Binding("IsLoading") { Source = _viewModel };
            BindingOperations.SetBinding(
                progressIndicator, ProgressIndicator.IsIndeterminateProperty, binding);

            progressIndicator.Text = "Loading new tweets..."; 
             * */
        }

        private async void Pivot_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Pivot Doubletapped");
            var statusBar = StatusBar.GetForCurrentView();
            await statusBar.ProgressIndicator.ShowAsync();
            await LoadData(true);
            await statusBar.ProgressIndicator.HideAsync();
        }






    }
}
