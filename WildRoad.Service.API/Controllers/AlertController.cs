using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using WildRoad.Business.BusinessLogic;
using WildRoadApp_Api.DataManager;

namespace WildRoad.Service.API.Controllers
{
    public class AlertController : ApiController
    {
        private BOAlert alertModel = new BOAlert();

        // GET api/values
        public async Task<IHttpActionResult> Get()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await alertModel.GetAllAlerts();
                return Ok(result);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> PostAlert([FromBody]ALERT value)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                alertModel.SaveAlert(value);
                return Ok();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                return BadRequest(ex.Message);
            }
        }

    }
}