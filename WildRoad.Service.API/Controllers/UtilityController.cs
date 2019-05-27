using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using WildRoad.Business.BusinessLogic;

namespace WildRoad.Service.API.Controllers
{
    public class UtilityController : ApiController
    {

        private BOIncident incidentModel = new BOIncident();

        // GET: api/Utility
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Utility/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Utility
        public async Task<IHttpActionResult> Post([FromUri]string account, [FromUri]string number)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                //incidentModel.SaveCredentials(account,number);
                return Ok();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/Utility/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Utility/5
        public void Delete(int id)
        {
        }
    }
}
