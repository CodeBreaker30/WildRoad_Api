using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using WildRoad.Business.BusinessLogic;

namespace WildRoad.Service.API.Controllers
{
    [RoutePrefix("api/parks")]
    public class NationalParkController : ApiController
    {
        private BONationalPark npLogic = new BONationalPark();
        /// <summary>
        /// Gets the menu list.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        //[Authorize]
        [HttpGet]
        public async Task<IHttpActionResult> GET()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await npLogic.GetNationalParks(-1);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [Route("parks")]
        [HttpGet]
        public async Task<IHttpActionResult> GET(int id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await npLogic.GetNationalParks(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                return BadRequest(ex.Message);
            }
        }

    }
}
