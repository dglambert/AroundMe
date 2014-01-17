﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace AroundMe
{
    public partial class SearchResults : PhoneApplicationPage
    {

        private double _latitude;
        private double _longitude;
        private string _topic;
        private double _radius;

        private const string flickrApiKey = "7a3ea25613dacffdd4296836f2bc09aa";


        public SearchResults()
        {
            InitializeComponent();

            Loaded += SearchResults_Loaded;

        }

        async void SearchResults_Loaded(object sender, RoutedEventArgs e)
        {
            //LocationTextBlock.Text = string.Format("Location: {0} & {1}", _latitude, _longitude);

            var images =  await FlickrImage.GetFlickrImages(flickrApiKey, _topic, _latitude, _longitude, _radius);

            DataContext = images;

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            _latitude = Convert.ToDouble(NavigationContext.QueryString["latitude"]);
            _longitude = Convert.ToDouble( NavigationContext.QueryString["longitude"]);
            _radius = Convert.ToDouble(NavigationContext.QueryString["radius"]);
            _topic = NavigationContext.QueryString["topic"];
        }

        private void LongListMultiSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void PhotosForLockscreen_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}