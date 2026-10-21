using System;
using System.Web.Security;
using PSCIMS.Shared;

namespace PSCIMS.jobs
{
    public partial class logout : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // 1. Prevent browser from caching the logout action or previous pages
            Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.AppendHeader("Pragma", "no-cache");

            // 2. Log the event
            int? applicantId = Session["ApplicantId"] as int?;
            if (applicantId.HasValue)
            {
                AuditLogger.Log("logout", applicantId.Value, true, "");
            }

            // 3. Clear Authentication Cookie
            FormsAuthentication.SignOut();

            // 4. Abandon Session
            Session.Clear();
            Session.Abandon();

            // 5. Expire the ASP.NET Session Cookie immediately
            if (Request.Cookies["ASP.NET_SessionId"] != null)
            {
                var cookie = new System.Web.HttpCookie("ASP.NET_SessionId", "")
                {
                    Expires = DateTime.Now.AddMonths(-20)
                };
                Response.Cookies.Add(cookie);
            }

            // 6. Expire the Forms Auth Cookie immediately
            if (Request.Cookies[FormsAuthentication.FormsCookieName] != null)
            {
                var cookie = new System.Web.HttpCookie(FormsAuthentication.FormsCookieName, "")
                {
                    Expires = DateTime.Now.AddMonths(-20)
                };
                Response.Cookies.Add(cookie);
            }

            // 7. Redirect to Login
            Response.Redirect("~/jobs/loginPage.aspx");
        }
    }
}