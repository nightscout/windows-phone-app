using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Xml.Linq;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using System.Diagnostics;
using Windows.Web.Syndication;
using Windows.Foundation;

    namespace NightScout.LiveTile
{
    public sealed class TileUpdater : IBackgroundTask
    {
        private string nightScoutJson = "";
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            try
            {

                string NightScoutJsonURL = "";
                Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

                if (localSettings.Values["NightScoutJson"] != null) {
                    NightScoutJsonURL = localSettings.Values["NightScoutJson"].ToString();

                    BackgroundTaskDeferral deferral = taskInstance.GetDeferral();
                    nightScoutJson = await getJsonUpdate(new Uri(NightScoutJsonURL));
                    NightScoutJsonFeed.updateTile(nightScoutJson);
                    // Inform the system that the task is finished.
                    deferral.Complete();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }


        public IAsyncOperation<String> getJsonUpdate(Uri URL)
        {
           
                HttpClient client = new HttpClient();
                Task<String> response  = client.GetStringAsync(URL);
                return (response.AsAsyncOperation<String>());
            
        }
    }
}

