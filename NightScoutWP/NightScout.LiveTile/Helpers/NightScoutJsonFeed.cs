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
using System.Diagnostics;
using Newtonsoft.Json.Converters;

namespace NightScout.LiveTile
{

    public sealed class NightScoutJsonFeed
    {

        static public void updateTile(string jSonResponse)
        {

            try
            {
                string bg = "";
                char direction = '-';
                string battery = "";

                JObject response = JObject.Parse(jSonResponse);
                string timeframe = JsonConvert.DeserializeObject(response["bgs"][0]["datetime"].ToString()).ToString();

                bg = response["bgs"][0]["sgv"].ToString();
                battery = response["bgs"][0]["battery"].ToString();


                if (response["bgs"][0]["direction"].ToString().ToLower().Contains("up"))
                    direction = '\x25B2';
                else
                    if (response["bgs"][0]["direction"].ToString().ToLower().Contains("down"))
                    direction = '\x25BC';

                string notification = String.Format("Current bg: {0} \r\nTrend: {1} \r\nBattery: {2}% ", bg, direction.ToString(), battery);

                XmlDocument tileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileWide310x150Text01);
                XmlNodeList tileTextAttributes = tileXml.GetElementsByTagName("text");
                tileTextAttributes[0].InnerText = notification;

                XmlDocument squareTileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquare150x150Text04);
                XmlNodeList squareTileTextAttributes = squareTileXml.GetElementsByTagName("text");
                squareTileTextAttributes[0].AppendChild(squareTileXml.CreateTextNode(notification));
                IXmlNode node = tileXml.ImportNode(squareTileXml.GetElementsByTagName("binding").Item(0), true);
                tileXml.GetElementsByTagName("visual").Item(0).AppendChild(node);

                TileNotification tileNotification = new TileNotification(tileXml);

                tileNotification.ExpirationTime = DateTimeOffset.UtcNow.AddSeconds(100);

                TileUpdateManager.CreateTileUpdaterForApplication().Update(tileNotification);

                XmlDocument badgeXml = BadgeUpdateManager.GetTemplateContent(BadgeTemplateType.BadgeGlyph);
                XmlElement badgeElement = (XmlElement)badgeXml.SelectSingleNode("/badge");
                badgeElement.SetAttribute("value", String.Format("Current bg: {0} \r\nTrend: {1}", bg, direction.ToString()));

                BadgeNotification badge = new BadgeNotification(badgeXml);
                BadgeUpdateManager.CreateBadgeUpdaterForApplication().Update(badge);

                int reading = System.Int32.Parse(bg);

                if ((reading > 200) || (reading < 80))
                {
                    ToastTemplateType toastTemplate = ToastTemplateType.ToastImageAndText01;
                    XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(toastTemplate);
                    XmlNodeList toastTextElements = toastXml.GetElementsByTagName("text");
                    toastTextElements[0].AppendChild(toastXml.CreateTextNode(String.Format("Current bg: {0} Trend: {1}", bg, direction.ToString())));                   
                                       
                    IXmlNode toastNode = toastXml.SelectSingleNode("/toast");
                    ((XmlElement)toastNode).SetAttribute("duration", "long");
                    XmlElement audio = toastXml.CreateElement("audio");
                    toastNode.AppendChild(audio);
                    
                    ToastNotification toast = new ToastNotification(toastXml);
                    ToastNotificationManager.CreateToastNotifier().Show(toast);
                }
            }
            catch (Exception)
            {
                throw;
            }

        }
    }
}
