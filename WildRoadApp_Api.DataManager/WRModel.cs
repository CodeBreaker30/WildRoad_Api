using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WildRoadApp_Api.DataManager
{
    public class WRModel
    {
        public WRModel()
        {

        }

        public List<NATIONAL_PARK> GetAllNationalParks(int parkId)
        {
            try
            {
                using (WildRoadDBEntities _wrEntities = new WildRoadDBEntities())
                {
                    return _wrEntities.NATIONAL_PARK.ToList();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        
        public List<HISTORY_REPORT> GetAllIncidents()
        {
            try
            {
                using (WildRoadDBEntities _wrEntities = new WildRoadDBEntities())
                {
                    return _wrEntities.HISTORY_REPORT.ToList();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        //public void SaveContact(string username, string phone)
        //{
        //    try
        //    {
        //        using (WildRoadDBEntities _wrEntities = new WildRoadDBEntities()) {
        //            CONTACT_DATA new_contact = new CONTACTS_DATA();
        //            new_contact.name = username;
        //            new_contact.phone = phone;

        //            _wrEntities.CONTACTS_DATA.Add(new_contact);
        //            _wrEntities.SaveChanges();
        //        }
        //    }
        //    catch (Exception e)
        //    {

        //        throw e;
        //    }
        //}

        public void SaveIncident(INCIDENT incident)
        {
            try
            {
                using (WildRoadDBEntities _wrEntities = new WildRoadDBEntities())
                {
                    INCIDENT new_incident = new INCIDENT();
                    new_incident = incident;

                    _wrEntities.INCIDENTs.Add(new_incident);
                    _wrEntities.SaveChanges();
                }
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        public void SaveAlert(ALERT alert)
        {
            try
            {
                using (WildRoadDBEntities _wrEntities = new WildRoadDBEntities())
                {
                    ALERT new_alert = new ALERT();
                    new_alert = alert;

                    _wrEntities.ALERTs.Add(new_alert);
                    _wrEntities.SaveChanges();
                }
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        public List<INCIDENT_TYPE> GetAllIncidentTypes() {
            List<INCIDENT_TYPE> result = new List<INCIDENT_TYPE>();
            try
            {
                using (WildRoadDBEntities _wrEntities = new WildRoadDBEntities())
                {

                    result = _wrEntities.INCIDENT_TYPE.ToList();
                }
            }
            catch (Exception e)
            {

                throw e;
            }

            return result;
        }

        public List<ALERT> GetAllAlerts()
        {
            List<ALERT> result = new List<ALERT>();
            try
            {
                using (WildRoadDBEntities _wrEntities = new WildRoadDBEntities())
                {

                    result = _wrEntities.ALERTs.ToList();
                }
            }
            catch (Exception e)
            {

                throw e;
            }

            return result;
        }

        public List<GEO_FENCE> GetAllIncidentGeofences() {
            List<GEO_FENCE> result = new List<GEO_FENCE>();
            try
            {
                using (WildRoadDBEntities _wrEntities = new WildRoadDBEntities())
                {

                    result = _wrEntities.GEO_FENCE.Where(f => f.fence_name != "geofence-1").ToList();
                }
            }
            catch (Exception e)
            {

                throw e;
            }

            return result;
        }
    }
}
