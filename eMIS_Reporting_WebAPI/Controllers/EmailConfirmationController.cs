using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using eMIS_Reporting_WebAPI.Models;
using Utilities;
using System.Configuration;

namespace eMIS_Reporting_WebAPI.Controllers
{
    public class EmailConfirmationController : ApiController
    {
        // GET: api/EmailConfirmation
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/EmailConfirmation/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/EmailConfirmation
        [HttpPost]
        public string Post([FromUri] string value)
        {
            try
            {
                SqlHelper sql = new SqlHelper();
                sql.AddGuidInputParam("@Guid", new Guid(value));
                sql.ExecuteNonQuery("spActivateEmail", ConfigurationManager.ConnectionStrings["eMIS_Reporting"].ToString());
                return "Success";
            }
            catch(Exception ex)
            {
                Logger.error(ex.Message, 0);
            }
            return "Error";
        }

        // PUT: api/EmailConfirmation/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/EmailConfirmation/5
        public void Delete(int id)
        {

        }
    }
}
