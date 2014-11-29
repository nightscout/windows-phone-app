using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Phone.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


// The WebView Application template is documented at http://go.microsoft.com/fwlink/?LinkID=391641

namespace NightScout.WindowsPhone
{

    public sealed partial class MainPage : Page
    {
        // TODO: Replace with your URL here.
        private string NightScoutURL;
        private Windows.Storage.ApplicationDataContainer localSettings;
        private Windows.Storage.StorageFolder localFolder;
        
        public MainPage()
        {
            this.InitializeComponent();
            localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

            if (localSettings.Values["NightScoutURL"] != null)
                NightScoutURL = localSettings.Values["NightScoutURL"].ToString();
            
            this.NavigationCacheMode = NavigationCacheMode.Required;               

        }




        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationMode== NavigationMode.Back)
                {
                if (localSettings.Values["NightScoutURL"] != null)
                    NightScoutURL = localSettings.Values["NightScoutURL"].ToString();
            }
            if (NightScoutURL!= null)
            {
                WebViewControl.Navigate(new Uri(NightScoutURL));
                HardwareButtons.BackPressed += this.MainPage_BackPressed;
                
            }
           
        }

        /// <summary>
        /// Invoked when this page is being navigated away.
        /// </summary>
        /// <param name="e">Event data that describes how this page is navigating.</param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed -= this.MainPage_BackPressed;
        }

        /// <summary>
        /// Overrides the back button press to navigate in the WebView's back stack instead of the application's.
        /// </summary>
        private void MainPage_BackPressed(object sender, BackPressedEventArgs e)
        {
            if (WebViewControl.CanGoBack)
            {
                WebViewControl.GoBack();
                e.Handled = true;
            }
        }

        private void Browser_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            if (!args.IsSuccess)
            {
                Debug.WriteLine("Navigation to this page failed, check your internet connection.");
            }
        }

        
        private void RefreshAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            WebViewControl.Refresh();       
        }

        
        private void SettingsAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Settings),this);
        }        

        }
        
    }

