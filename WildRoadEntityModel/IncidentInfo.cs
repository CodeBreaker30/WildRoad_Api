using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WildRoadEntityModel
{
    public class IncidentInfo
    {
        public string accident_date { get; set; }
        public string accident_time { get; set; }
        public string light_condition_desc { get; set; }
        public int severity { get; set; }
        public string day_of_week { get; set; }
        public int speed_zone { get; set; }
        public string lg_area_name { get; set; }
        public string region_name { get; set; }
        public string deg_urban_name { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public int postcode { get; set; }
        public double riskGrade { get; set; }
    }
}
