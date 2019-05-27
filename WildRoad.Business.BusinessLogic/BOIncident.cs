using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WildRoad.Tools.Utilities;
using WildRoadApp_Api.DataManager;
using WildRoadEntityModel;

namespace WildRoad.Business.BusinessLogic
{

    public class BOIncident
    {
        public static WRModel wrModel = new WRModel();
        public static List<IncidentInfo> incidents = new List<IncidentInfo>();
        public static string fcmServerKey = string.Empty;
        public static string fcmSenderId = string.Empty;
        public static List<ConnectedUser> activeUsers = new List<ConnectedUser>();

        public BOIncident() {
            fcmServerKey = ConfigurationManager.AppSettings["FCMServerKey"].ToString();
            fcmSenderId = ConfigurationManager.AppSettings["FCMSenderID"].ToString();
            activeUsers = (List<ConnectedUser>)RuntimeCache.Instance.GetItem("activeusers");
        }

        public Task<List<IncidentInfo>> GetAllIncidents()
        {
            incidents.Clear();
            foreach (var item in wrModel.GetAllIncidents())
            {
                incidents.Add(new IncidentInfo()
                {
                    accident_date = item.accident_date,
                    deg_urban_name = item.deg_urban_name,
                    accident_time = item.accident_time,
                    latitude = item.latitude,
                    longitude = item.longitude,
                    lg_area_name = item.lg_area_name,
                    light_condition_desc = item.light_condition_desc,
                    postcode = item.postcode,
                    region_name = item.region_name,
                    severity = item.severity,
                    speed_zone = item.speed_zone,
                    day_of_week = item.day_week_desc,
                    riskGrade = GetRiskOfGrade(item.accident_date, item.accident_time, item.light_condition_desc, item.day_week_desc, item.severity)
                });
            }

            return Task.Run(() => incidents);
        }

        public Task<List<IncidentInfo>> GetAllIncidentsBySuburbs(string suburbs)
        {
            List<string> listSub = suburbs.Split(',').ToList();
            List<IncidentInfo> result = new List<IncidentInfo>();

            foreach (var item in wrModel.GetAllIncidents())
            {
                result.Add(new IncidentInfo()
                {
                    accident_date = item.accident_date,
                    deg_urban_name = item.deg_urban_name,
                    accident_time = item.accident_time,
                    latitude = item.latitude,
                    longitude = item.longitude,
                    lg_area_name = item.lg_area_name,
                    light_condition_desc = item.light_condition_desc,
                    postcode = item.postcode,
                    region_name = item.region_name,
                    severity = item.severity,
                    speed_zone = item.speed_zone,
                    riskGrade = GetRiskOfGrade(item.accident_date, item.accident_time, item.light_condition_desc, item.day_week_desc, item.severity)
                });
            }
            List<IncidentInfo> filteredResult = new List<IncidentInfo>();

            if (listSub.Count > 1)
            {
                filteredResult = result.Where(i => i.postcode == int.Parse(listSub[0]) || i.postcode == int.Parse(listSub[1])).ToList();
            } else {
                filteredResult = result.Where(i => i.postcode == int.Parse(listSub[0])).ToList();
            }  

            return Task.Run(() => filteredResult);
        }

        public Task<List<IncidentInfo>> GetNearByIncidentsByRoute(List<Position> route)
        {
            List<IncidentInfo> points = new List<IncidentInfo>();
            double avg = 0.0d;
            double count = 0.0d;
            double sum = 0.0d;

            BindIncidents();

            Parallel.ForEach(route, n =>
            {
                if (route.IndexOf(n) < route.Count - 1)
                {
                    Position nextNode = route[route.IndexOf(n) + 1];
                    double noded = GetMetersDistanceBetweenCoordinates(route[route.IndexOf(n)], nextNode);
                    //Position mp = getMiddlePoint(n, nextNode);

                    Parallel.ForEach(incidents, i =>
                    {
                        Position coord = new Position();
                        coord.Latitude = Double.Parse(i.latitude);
                        coord.Longitude = Double.Parse(i.longitude);

                        if (GetMetersDistanceBetweenCoordinates(nextNode, coord) <= (1000))
                        {
                            points.Add(i);
                        }
                    });
                }
            });

            return Task.Run(() => points);
        }

        public Task<RouteInsight> GetNearByIncidentsInsights(List<Position> route)
        {
            RouteInsight result = new RouteInsight();
            List<IncidentInfo> points = new List<IncidentInfo>();
            List<string> hours = new List<string>();
            List<string> dows = new List<string>();
            List<int> svts = new List<int>();
            double avg = 0.0d;
            double count = 0.0d;
            double sum = 0.0d;

            BindIncidents();

            Parallel.ForEach(route, n =>
            {
                if (route.IndexOf(n) < route.Count - 1)
                {
                    Position nextNode = route[route.IndexOf(n) + 1];
                    double noded = GetMetersDistanceBetweenCoordinates(route[route.IndexOf(n)], nextNode);
                    //Position mp = getMiddlePoint(n, nextNode);

                    Parallel.ForEach(incidents, i =>
                    {
                        Position coord = new Position();
                        coord.Latitude = Double.Parse(i.latitude);
                        coord.Longitude = Double.Parse(i.longitude);

                        if (GetMetersDistanceBetweenCoordinates(nextNode, coord) <= (1000))
                        {
                            hours.Add(GetRiskOfHour(int.Parse(i.accident_time.Substring(0, 2))));
                            dows.Add(i.day_of_week);
                            svts.Add(i.severity);
                            sum = sum + i.riskGrade;
                            count++;
                            //points.Add(i);
                        }
                    });
                }
            });

            avg = sum / count;
            int mstsvt = 0;
            string msthour = "";
            string mstdow = "";


            if (hours.Count > 0)
            {
                msthour = (from i in hours
                                  group i by i into grp
                                  orderby grp.Count() descending
                                  select grp.Key).First();
            }

            if (dows.Count > 0) {
                mstdow = (from i in dows
                                 group i by i into grp
                                 orderby grp.Count() descending
                                 select grp.Key).First();
            }

            if (svts.Count > 0) {
                mstsvt = (from i in svts
                              group i by i into grp
                              orderby grp.Count() descending
                              select grp.Key).First();
            }
            

            result = new RouteInsight()
            {
                DayOfWeek = string.IsNullOrEmpty(mstdow)?"None":mstdow,
                HourOfDay = string.IsNullOrEmpty(msthour) ? "None" : msthour,
                Severity = mstsvt,
                RiskOfGrade = Double.IsNaN(avg)?0:(int)avg
            };

            return Task.Run(() => result);
        }

        public Task<List<IncidentFence>> GetAllGeoFences()
        {
            List<IncidentFence> fences = new List<IncidentFence>();
            List<GEO_FENCE> fence_query = wrModel.GetAllIncidentGeofences();
            for (int i = 0; i<fence_query.Count; i++)
            {
                List<Position> route = new List<Position>();
                string fence_name = fence_query[i].fence_name;

                int exist = fences.Where(f => f.FenceName.Equals(fence_name)).ToList().Count;
                if (exist > 1) continue;

                route.Add(new Position()
                {
                    Latitude = Double.Parse(fence_query[i].latitude),
                    Longitude = Double.Parse(fence_query[i].longitude)
                });

                for (int j = i+1; j< fence_query.Count; j++)
                {
                    if (fence_name.Equals(fence_query[j].fence_name)) {
                        route.Add(new Position()
                        {
                            Latitude = Double.Parse(fence_query[j].latitude),
                            Longitude = Double.Parse(fence_query[j].longitude)
                        });
                    }
                }

                fences.Add(new IncidentFence()
                {
                    FenceName = fence_name,
                    Route = route
                });
            }
            return Task.Run(() => fences);
        }

        public double GetRiskOfGrade(string l, string h, string c, string d, int s)
        {
            double date = GetRiskOfLatest(l);
            double hour = GetRiskOfHour(h);
            double light = GetRiskOfLight(c);
            double dow = GetRiskDayOfWeek(d);
            double sev = GetRiskOfSeverity(s);

            return (3.4*date) + (2.6*hour) + (2*light) + (1.3*dow) + (0.7*sev);
        }

        public double GetRiskOfLatest(string date) {
            return 1;
        }

        public double GetRiskOfHour(string hour) {
            int hd = int.Parse(hour.Substring(0,2));

            if (hd <= 6)
                return 0.25;

            if (hd > 6 && hd <= 12)
                return 0.75;

            if (hd > 12 && hd <= 18)
                return 0.5;

            if (hd > 18 && hd <= 24)
                return 1.0;


            return 0.0;
        }

        public string GetRiskOfHour(int hd)
        {
            //int hd = int.Parse(hour.Substring(0, 2));

            if (hd <= 6)
                return "Dawn";

            if (hd > 6 && hd <= 12)
                return "Day";

            if (hd > 12 && hd <= 18)
                return "Afternoon";

            if (hd > 18 && hd <= 24)
                return "Night";


            return "Day";
        }

        public double GetRiskOfLight(string light_cond) {
            switch (light_cond) {
                case "Dark No street lights": //Dark No Street
                    return 1.0;
                case "Dark Street lights off": //Dark Lights Off
                    return 0.15;
                case "Dark Street lights unknown": //Dark Lights Unknown
                    return 0.29;
                case "Dark Street lights on": //Dark Lights On
                    return 0.57;
                case "Day": //Day
                    return 0.85;
                case "Dusk/Dawn": //Dusk
                    return 0.71;
                case "Unknown": //Unknown
                    return 0.43;
                default:
                    return 0.0;
            }
        }

        public double GetRiskDayOfWeek(string dow) {
            switch (dow) {
                case "Sunday":
                    return 0.85;
                case "Monday":
                    return 0.57;
                case "Tuesday":
                    return 0.15;
                case "Wednesday":
                    return 0.29;
                case "Thursday":
                    return 0.43;
                case "Friday":
                    return 0.71;
                case "Saturday":
                    return 1.0;
                default:
                    return 0.0;
            }
        }

        public double GetRiskOfSeverity(int severity) {
            switch (severity) {
                case 1:
                    return 1.0;
                case 2:
                    return 0.66;
                case 3:
                    return 0.33;
                default:
                    return 0.0;
            }
        }

        public void BindIncidents() {
            if(incidents.Count == 0)
            {
                foreach (var item in wrModel.GetAllIncidents())
                {
                    incidents.Add(new IncidentInfo()
                    {
                        accident_date = item.accident_date,
                        deg_urban_name = item.deg_urban_name,
                        accident_time = item.accident_time,
                        latitude = item.latitude,
                        longitude = item.longitude,
                        lg_area_name = item.lg_area_name,
                        light_condition_desc = item.light_condition_desc,
                        postcode = item.postcode,
                        region_name = item.region_name,
                        day_of_week = item.day_week_desc,
                        severity = item.severity,
                        speed_zone = item.speed_zone,
                        riskGrade = GetRiskOfGrade(item.accident_date, item.accident_time, item.light_condition_desc, item.day_week_desc, item.severity)
                    });
                }
            }

        }

        public void GetNearByIncidentsByPosition(string value) {
            List<string> posstr = value.Split(',').ToList();
            double sum = 0.0;
            double count = 0.0;
            double avg = 0.0;
            double clstdst = 1000000000.0;
            string userToken = posstr[2].Substring(1, posstr[2].Length - 2);
            Position position = new Position()
            {
                Latitude = Double.Parse(posstr[0]),
                Longitude = Double.Parse(posstr[1])
            };

            AddNewUserToCache(new ConnectedUser()
            {
                UserToken = userToken,
                Position = position
            });

            BindIncidents();

            Parallel.ForEach(incidents, i =>
            {
                Position in_pos = new Position()
                {
                    Latitude = Double.Parse(i.latitude),
                    Longitude = Double.Parse(i.longitude)
                };
                double currentDst = GetMetersDistanceBetweenCoordinates(position, in_pos);
                if (currentDst < 4000)
                {
                    clstdst = currentDst < clstdst ? (currentDst > 0 ? currentDst : clstdst) : clstdst;
                    sum = sum + i.riskGrade;
                    count++;
                }
            });

            avg = sum / count;
            if (!Double.IsNaN(avg))
            {
                PublishCloudMessageToApp(posstr[2].Substring(1, posstr[2].Length - 2), GetTextAnalysisFromAlerts((int)avg, clstdst));
            }
            
        }

        public void AddNewUserToCache(ConnectedUser user) {
            string key = String.Concat("user", user.UserToken.Substring(0, 10));

            if (!RuntimeCache.Instance.ExistItem(key)) {
                RuntimeCache.Instance.AddItem(key, user);
            }
        }

        public double GetMetersDistanceBetweenCoordinates(Position a, Position b) {
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

        public string GetTextAnalysisFromAlerts(int riskGrade, double closestDistance) {
            return String.Concat("There is a ",
                                    riskGrade > 80 ? "high" : riskGrade > 40 ? "medium" : "considerable",
                                    " risk of incidents with wildlife in approx ",
                                    Convert.ToString((int)(closestDistance / 1000)),
                                    " kms.");
        }

        public void PublishCloudMessageToApp(string destToken, string message) {
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

        //public void SaveCredentials(string user, string phone)
        //{
        //    wrModel.SaveContact(user, phone); 
        //}

        public void SaveIncident(NewIncident newIncident)
        {
            INCIDENT_TYPE in_type = wrModel.GetAllIncidentTypes()
                .Where(i => i.type_description == newIncident.IncidentType)
                .FirstOrDefault();

            wrModel.SaveIncident(new INCIDENT() {
                animal = newIncident.Animal,
                comments = newIncident.Observations,
                incident_date= newIncident.IssueDate,
                incident_type= in_type.type_no,
                latitude= newIncident.Latitude,
                longitude= newIncident.Longitude,
                people_hurt= (short)(newIncident.IsPeopleHurt?1:0),
                suburb= newIncident.Suburb,
                userId_report = newIncident.UseridReport
            });

            new BOAlert().SaveAlert(new ALERT()
            {
                date_time = newIncident.IssueDate,
                latitude = newIncident.Latitude,
                longitude = newIncident.Longitude,
                reporter = newIncident.UseridReport
            });
        }
    }


}
