// Key 7a3ea25613dacffdd4296836f2bc09aa
// Secret 1fc44f1d4c13ba26

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using AroundMe.Resources;
using System.Device.Location;
using Windows.Devices.Geolocation;
using System.Net.Http;
using Newtonsoft.Json;
using System.Windows.Media.Imaging;

namespace AroundMe
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            Loaded += MainPage_Loaded;

            // Sample code to localize the ApplicationBar
            BuildLocalizedApplicationBar();
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            SystemTray.ProgressIndicator = new ProgressIndicator();

            UpdateMap();
        }

        private static void SetProgressIndicator(bool isVisible)
        {
            SystemTray.ProgressIndicator.IsIndeterminate = isVisible;
            SystemTray.ProgressIndicator.IsVisible = isVisible;
        }

        private async void UpdateMap()
        {
            Geolocator geolocator = new Geolocator();
            geolocator.DesiredAccuracyInMeters = 50;

            SetProgressIndicator(true);
            SystemTray.ProgressIndicator.Text = "Getting GPS Location";

            try
            {
                Geoposition position =
                await geolocator.GetGeopositionAsync(TimeSpan.FromMinutes(1), TimeSpan.FromSeconds(30));

                SystemTray.ProgressIndicator.Text = "Acquired";

                var gpsCoorCenter =
                    new GeoCoordinate(position.Coordinate.Latitude, position.Coordinate.Longitude);


                ///////////// Coords for John Hancock Center Chicago /////////
                //var gpsCoorCenter = new GeoCoordinate(41.8988, -87.6123);

                
                AroundMeMap.Center = gpsCoorCenter;
                AroundMeMap.ZoomLevel = 15;
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Location is disabled in phone settings.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                SetProgressIndicator(false);
            }
        }

        // Sample code for building a localized ApplicationBar
        private void BuildLocalizedApplicationBar()
        {
            // Set the page's ApplicationBar to a new instance of ApplicationBar.
            ApplicationBar = new ApplicationBar();

            // Create a new button and set the text value to the localized string from AppResources.
            ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/feature.search.png", UriKind.Relative));
            appBarButton.Text = AppResources.AppBarButtonText;
            appBarButton.Click += SearchClick;
            ApplicationBar.Buttons.Add(appBarButton);

            // Create a new menu item with the localized string from AppResources.
            ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
            ApplicationBar.MenuItems.Add(appBarMenuItem);
        }

        private void SearchClick(object sender, EventArgs e)
        {
            string topic = HttpUtility.UrlEncode(SearchTopic.Text);

            string navTo = string.Format(   "/SearchResults.xaml?latitude={0}&longitude={1}&topic={2}&radius={3}", 
                                            AroundMeMap.Center.Latitude, AroundMeMap.Center.Longitude, topic, 5);
            NavigationService.Navigate(new Uri(navTo, UriKind.RelativeOrAbsolute));
        }
    }
}