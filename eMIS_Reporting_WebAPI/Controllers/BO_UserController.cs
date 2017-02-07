using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using System.Web.Http.OData;
using System.Web.Http.OData.Routing;
using eMIS_Reporting_WebAPI.Models;
using System.Net.Mail;
using System.Configuration;
using Utilities;

namespace eMIS_Reporting_WebAPI.Controllers
{
    /*
    The WebApiConfig class may require additional changes to add a route for this controller. Merge these statements into the Register method of the WebApiConfig class as applicable. Note that OData URLs are case sensitive.

    using System.Web.Http.OData.Builder;
    using System.Web.Http.OData.Extensions;
    using eMIS_Reporting_WebAPI.Models;
    ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
    builder.EntitySet<BO_User>("BO_User");
    config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class BO_UserController : ODataController
    {
        private eMIS_ReportingEntities db = new eMIS_ReportingEntities();

        // GET: odata/BO_User
        [EnableQuery]
        public IQueryable<BO_User> GetBO_User()
        {
            return db.BO_User;
        }

        // GET: odata/BO_User(5)
        [EnableQuery]
        public IQueryable<BO_User> GetBO_User([FromODataUri] string Name, [FromODataUri] string Surname, [FromODataUri] string Email, [FromODataUri] string ContactNumber, [FromODataUri] string Municipality, [FromODataUri] string Department, [FromODataUri] string Password)
        {
            var result = db.spInsertNewUser(Name, Surname, Password, Email, ContactNumber, Department, Municipality, "");
            var a = result.FirstOrDefault();
            if (Convert.ToInt32(result.FirstOrDefault()) == 0)
            {
                return null;
            }
            else
            {
                MailMessage email = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("mail.software-solutions.co.za");
                email.From = new MailAddress(ConfigurationManager.AppSettings["FromEmailAddress"].ToString());
                email.To.Add(Email);
                email.Subject = "eMIS Activation";
                email.Body = "Welcome to emis follow the link to activate your email";

                //System.Net.Mail.Attachment attachment;
                //attachment = new Attachment(path + Convert.ToString(id) + quoteorinvoice + time + ".pdf");
                //email.Attachments.Add(attachment);

                SmtpServer.Port = 25;
                SmtpServer.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["FromEmailAddress"].ToString(), ConfigurationManager.AppSettings["FromEmailAddressPassword"].ToString());
                SmtpServer.EnableSsl = false;

                SmtpServer.Send(email);
                return db.BO_User.Where(val => val.Email == Email).AsQueryable();
            }
        }

        // PUT: odata/BO_User(5)
        public async Task<IHttpActionResult> Put([FromODataUri] Guid key, Delta<BO_User> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            BO_User bO_User = await db.BO_User.FindAsync(key);
            if (bO_User == null)
            {
                return NotFound();
            }

            patch.Put(bO_User);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BO_UserExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(bO_User);
        }

        // POST: odata/BO_User
        public async Task<IHttpActionResult> Post(BO_User bO_User)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                var username = bO_User.Name + bO_User.Surname[0];
                bO_User.User_Name = username;

                db.BO_User.Add(bO_User);
                await db.SaveChangesAsync();
                MailMessage email = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("mail.software-solutions.co.za");
                email.From = new MailAddress(ConfigurationManager.AppSettings["FromEmailAddress"].ToString());
                email.To.Add(bO_User.Email);
                email.Subject = "eMIS Activation";
                SqlHelper sql = new SqlHelper();
                sql.AddVarCharInputParam("@username", username, 100);
                sql.AddVarCharInputParam("@password", bO_User.Password, 100);
                DataSet ds = sql.GetDataSet("spGetUserByEmailPassword", ConfigurationManager.ConnectionStrings["eMIS_Reporting"].ToString());
                email.Body = "Welcome to emis follow the link to activate your email" + Environment.NewLine + Environment.NewLine + "http://localhost:63416/EmailConfirmationPage.html?email=" + ds.Tables[0].Rows[0][0];

                //System.Net.Mail.Attachment attachment;
                //attachment = new Attachment(path + Convert.ToString(id) + quoteorinvoice + time + ".pdf");
                //email.Attachments.Add(attachment);

                SmtpServer.Port = 25;
                SmtpServer.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["FromEmailAddress"].ToString(), ConfigurationManager.AppSettings["FromEmailAddressPassword"].ToString());
                SmtpServer.EnableSsl = false;

                SmtpServer.Send(email);
            }
            return Created(bO_User);
        }

        // PATCH: odata/BO_User(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] Guid key, Delta<BO_User> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            BO_User bO_User = await db.BO_User.FindAsync(key);
            if (bO_User == null)
            {
                return NotFound();
            }

            patch.Patch(bO_User);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BO_UserExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(bO_User);
        }

        // DELETE: odata/BO_User(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            BO_User bO_User = await db.BO_User.FindAsync(key);
            if (bO_User == null)
            {
                return NotFound();
            }

            db.BO_User.Remove(bO_User);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool BO_UserExists(Guid key)
        {
            return db.BO_User.Count(e => e.User_ID == key) > 0;
        }
    }
}
