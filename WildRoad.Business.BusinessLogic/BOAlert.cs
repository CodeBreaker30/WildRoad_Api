using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using WildRoad.Tools.Utilities;
using WildRoadApp_Api.DataManager;
using WildRoadEntityModel;

namespace WildRoad.Business.BusinessLogic
{
    public class BOAlert
    {
        public static WRModel wrModel = new WRModel();
        public static List<ALERT> alerts = new List<ALERT>();
        public static string fcmServerKey = string.Empty;
        public static string fcmSenderId = string.Empty;
        public static string consumerKey = string.Empty;
        public static string consumerKeySecret = string.Empty;
        public static string accessToken = string.Empty;
        public static string accessTokenSecret = string.Empty;
        static string baseUri = "https://maps.googleapis.com/maps/api/geocode/xml?latlng={0},{1}&sensor=false&key=AIzaSyAmXumMFWYDCpgqA-EzUlBYcF5HuxsOjQ8";

        public BOAlert()
        {
            fcmServerKey = ConfigurationManager.AppSettings["FCMServerKey"].ToString();
            fcmSenderId = ConfigurationManager.AppSettings["FCMSenderID"].ToString();
            consumerKey = ConfigurationManager.AppSettings["TwConsumerKey"].ToString();
            consumerKeySecret = ConfigurationManager.AppSettings["TwConsumerKeySecret"].ToString();
            accessToken = ConfigurationManager.AppSettings["TwAccessToken"].ToString();
            accessTokenSecret = ConfigurationManager.AppSettings["TwAccessTokenSecret"].ToString();
        }

        public void SaveAlert(ALERT alert)
        {
            AlertActiveUsers(alert);
            PublishTweet(alert);
            wrModel.SaveAlert(alert);   
        }

        public Task<List<ALERT>> GetAllAlerts()
        {
            alerts.Clear();
            alerts = wrModel.GetAllAlerts();

            return Task.Run(() => alerts);
        }

        public void AlertActiveUsers(ALERT alert) {
            List<ConnectedUser> users = GetActiveUsers();
            Position alertLocation = new Position() {
                Latitude = Double.Parse(alert.latitude),
                Longitude = Double.Parse(alert.longitude)
            };
            if (users.Count == 0) return;

            foreach (ConnectedUser item in users)
            {
                if (item.UserToken == alert.reporter) continue;
                double diffDist = GetMetersDistanceBetweenCoordinates(item.Position, alertLocation);
                if (diffDist < 20000) {
                    PublishCloudMessageToApp(item.UserToken, "An incident was reported "+((int)(diffDist/1000))+" Kms nearby from your position, please be careful");
                }
            }
        }

        public List<ConnectedUser> GetActiveUsers() {
            List<ConnectedUser> result = new List<ConnectedUser>();
            foreach (var item in RuntimeCache.Instance.GetAllElementsFromMemory())
            {
                if (item.Key.ToLower().Contains("user")) {
                    result.Add((ConnectedUser)item.Value);
                }
            }

            return result;
        }

        public double GetMetersDistanceBetweenCoordinates(Position a, Position b)
        {
            float pk = (float)(180.0 / Math.PI);

            float a1 = (float)a.Latitude / pk;
            float a2 = (float)a.Longitude / pk;
            float b1 = (float)b.Latitude / pk;
            float b2 = (float)b.Longitude / pk;

            double t1 = Math.Cos(a1) * Math.Cos(a2) * Math.Cos(b1) * Math.Cos(b2);
            double t2 = Math.Cos(a1) * Math.Sin(a2) * Math.Cos(b1) * Math.Sin(b2);
            double t3 = Math.Sin(a1) * Math.Sin(b1);
            double tt = Math.Acos(t1 + t2 + t3);

            return 6366000 * tt;
        }

        public void PublishCloudMessageToApp(string destToken, string message)
        {
            WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
            tRequest.Method = "post";
            //serverKey - Key from Firebase cloud messaging server  
            tRequest.Headers.Add(string.Format("Authorization: key={0}", fcmServerKey));
            //Sender Id - From firebase project setting  
            tRequest.Headers.Add(string.Format("Sender: id={0}", fcmSenderId));
            tRequest.ContentType = "application/json";
            var payload = new
            {
                to = destToken,
                priority = "high",
                content_available = true,
                notification = new
                {
                    body = message,
                    title = "New WildLife Alert",
                    badge = 1
                },
            };

            string postbody = JsonConvert.SerializeObject(payload).ToString();
            Byte[] byteArray = Encoding.UTF8.GetBytes(postbody);
            tRequest.ContentLength = byteArray.Length;
            using (Stream dataStream = tRequest.GetRequestStream())
            {
                dataStream.Write(byteArray, 0, byteArray.Length);
                using (WebResponse tResponse = tRequest.GetResponse())
                {
                    using (Stream dataStreamResponse = tResponse.GetResponseStream())
                    {
                        if (dataStreamResponse != null)
                        {
                            using (StreamReader tReader = new StreamReader(dataStreamResponse))
                            {
                                String sResponseFromServer = tReader.ReadToEnd();
                                //result.Response = sResponseFromServer;
                            }
                        }
                    }
                }
            }
        }

        public void PublishTweet(ALERT alert) {
            string location = RetrieveFormatedAddress(alert.latitude, alert.longitude);
            var twitter = new TwitterApiClientHandler(consumerKey, consumerKeySecret, accessToken, accessTokenSecret);
            var response = twitter.Tweet("A wildlife based incident has been reported at "
                                                    +location
                                                    +", "+alert.date_time
                                                    +". Retweet and share this with your friends to make them aware.");
        }

        public string RetrieveFormatedAddress(string lat, string lng)
        {
            string requestUri = string.Format(baseUri, lat, lng);

            using (WebClient wc = new WebClient())
            {
                string result = wc.DownloadString(requestUri);
                var xmlElm = XElement.Parse(result);
                var status = (from elm in xmlElm.Descendants()
                              where
                                   elm.Name == "status"
                              select elm).FirstOrDefault();
                if (status.Value.ToLower() == "ok")
                {
                    var res = (from elm in xmlElm.Descendants()
                               where
                                    elm.Name == "formatted_address"
                               select elm).FirstOrDefault();
                    requestUri = res.Value;

                    return requestUri;
                }
            }

            return string.Empty;
        }
    }
}
