using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Xml.Linq;
using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;

namespace NightScoutWP.Helpers
{
    public class NightScoutJsonFeed
    {
        static String nightScoutJson = "";

        
        public static async void getJsonUpdate(Uri URL)
        {
            HttpClient client = new HttpClient();
            nightScoutJson = await client.GetStringAsync(URL);
            JObject response = JObject.Parse(nightScoutJson);

            string notification = String.Format("Current bg: {0} \r\nTrend: {1} \r\nBattery: {2}% ", response["bgs"][0]["sgv"],response["bgs"][0]["direction"],response["bgs"][0]["battery"]);
            XmlDocument tileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileWide310x150ImageAndText01);

            XmlNodeList tileTextAttributes = tileXml.GetElementsByTagName("text");
            tileTextAttributes[0].InnerText = "NightScout";

            XmlNodeList tileImageAttributes = tileXml.GetElementsByTagName("image");
            ((XmlElement)tileImageAttributes[0]).SetAttribute("src", "ms-appx:///assets/logomobile.png");
            ((XmlElement)tileImageAttributes[0]).SetAttribute("alt", "red graphic");

            XmlDocument squareTileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquare150x150Text04);
            XmlNodeList squareTileTextAttributes = squareTileXml.GetElementsByTagName("text");
            squareTileTextAttributes[0].AppendChild(squareTileXml.CreateTextNode(notification));
            IXmlNode node = tileXml.ImportNode(squareTileXml.GetElementsByTagName("binding").Item(0), true);
            tileXml.GetElementsByTagName("visual").Item(0).AppendChild(node);

            TileNotification tileNotification = new TileNotification(tileXml);

            tileNotification.ExpirationTime = DateTimeOffset.UtcNow.AddSeconds(100);

            TileUpdateManager.CreateTileUpdaterForApplication().Update(tileNotification);


        }
    }
}
