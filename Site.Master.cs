using System;
using System.IO;
using System.Web;
using System.Web.Security;

namespace PSCIMS
{
    public partial class Site : System.Web.UI.MasterPage
    {
        public bool IsAuthenticated
        {
            get
            {
                return Request.IsAuthenticated
                    && Session != null
                    && Session["ApplicantId"] != null;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // Toggle Sidebar Navigation
            phAnonymous.Visible = !IsAuthenticated;
            phAuthenticated.Visible = IsAuthenticated;

            // Toggle Header Login/Logout Buttons
            phHeaderAnonymous.Visible = !IsAuthenticated;
            phHeaderAuthenticated.Visible = IsAuthenticated;

            //Enforce no-cache headers on ALL pages using this master page
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetValidUntilExpires(false);
            Response.AppendHeader("Pragma", "no-cache");
        }

        //Sidebar active-link helpers
        protected string IsActive(string pageName)
        {
            string current = Path.GetFileNameWithoutExtension(
                Request.AppRelativeCurrentExecutionFilePath);
            return string.Equals(current, pageName, StringComparison.OrdinalIgnoreCase)
                ? "active" : "";
        }

        protected string IsActiveIn(string folder, string pageName)
        {
            string currentFile = Path.GetFileNameWithoutExtension(
                Request.AppRelativeCurrentExecutionFilePath);
            string currentFolder = Path.GetFileName(
                Path.GetDirectoryName(Request.AppRelativeCurrentExecutionFilePath) ?? "");

            if (!string.Equals(currentFile, pageName, StringComparison.OrdinalIgnoreCase))
                return "";
            if (!string.Equals(currentFolder, folder, StringComparison.OrdinalIgnoreCase))
                return "";

            return "active";
        }

        protected string CurrentSection
        {
            get
            {
                string path = (Request.AppRelativeCurrentExecutionFilePath ?? "").ToLowerInvariant();
                if (path.Contains("/puio/")) return "puio";
                return "jobs";
            }
        }

        protected string CssVersion(string appRelativePath)
        {
            try
            {
                string physical = Server.MapPath(appRelativePath);
                if (System.IO.File.Exists(physical))
                {
                    var ticks = System.IO.File.GetLastWriteTimeUtc(physical).Ticks;
                    return ticks.ToString("x");
                }
            }
            catch { }
            return "1";
        }
    }
}