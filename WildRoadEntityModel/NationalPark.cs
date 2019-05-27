using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WildRoadEntityModel
{
    public class NationalPark
    {
        public int Np_id { get; set; }
        public string Np_name { get; set; }
        public string Np_description { get; set; }
        public string Np_longitude { get; set; }
        public string Np_latitude { get; set; }
        public int Np_frequency { get; set; }
        public string Np_picture_link { get; set; }

        public NationalPark()
        {

        }
    }
}
