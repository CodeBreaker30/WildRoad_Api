using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WildRoadApp_Api.DataManager;

namespace WildRoad.Business.BusinessLogic
{
    public class BONationalPark
    {
            public static WRModel wrModel = new WRModel();

            public Task<List<NATIONAL_PARK>> GetNationalParks(int parkId)
            {
                return Task.Run(() => wrModel.GetAllNationalParks(parkId));
            }
        }
}
