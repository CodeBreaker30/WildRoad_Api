using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using WildRoad.Business.BusinessLogic;
using WildRoadEntityModel;

namespace WildRoad.Service.API.Controllers
{
    public class IncidentController : ApiController
    {
        private BOIncident incidentModel = new BOIncident();
        // GET api/values
        public async Task<IHttpActionResult> Get()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await incidentModel.GetAllIncidents();
                return Ok(result);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetAllGeoFences() {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await incidentModel.GetAllGeoFences();
                return Ok(result);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        // GET api/values/5
        public async Task<IHttpActionResult> Get([FromUri]string suburb)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await incidentModel.GetAllIncidentsBySuburbs(suburb);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        // POST api/values
        [HttpPost]
        public async Task<IHttpActionResult> Post([FromUri]string value)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                incidentModel.GetNearByIncidentsByPosition(value);
                return Ok();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> PostLocations([FromBody]List<Position> route)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = new List<IncidentInfo>();
                   result = await incidentModel.GetNearByIncidentsByRoute(route);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> PostInsights([FromBody]List<Position> route)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = new RouteInsight();
                result = await incidentModel.GetNearByIncidentsInsights(route);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> PostIncident([FromBody]NewIncident value)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                incidentModel.SaveIncident(value);
                return Ok("{'response':'successful'}");
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
