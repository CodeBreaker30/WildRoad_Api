using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WildRoadEntityModel
{
    public class NewIncident
    {
        public String PlaceName { get; set; }
        public String Latitude { get; set; }
        public String Longitude { get; set; }
        public String Suburb { get; set; }
        public bool IsPeopleHurt { get; set; }
        public bool AnimalFatality { get; set; }
        public String Observations { get; set; }
        public String Animal { get; set; }
        public String IssueDate { get; set; }
        public int Postcode { get; set; }
        public String IncidentType { get; set; }
        public String UseridReport { get; set; }
    }
}
