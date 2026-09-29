using System;
using System.IO;

namespace PSCIMS
{
    public partial class Site : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e) { }

        /// <summary>
        /// True when the current page matches the filename, regardless of folder.
        /// Use for unique filenames only (e.g. loginPage, RegisterProfile).
        /// </summary>
        protected string IsActive(string pageName)
        {
            string current = Path.GetFileNameWithoutExtension(
                Request.AppRelativeCurrentExecutionFilePath);
            return string.Equals(current, pageName, StringComparison.OrdinalIgnoreCase)
                ? "active" : "";
        }

        /// <summary>
        /// True when both the folder AND the filename match.
        /// Use for Default.aspx which exists in multiple folders.
        /// </summary>
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

        /// <summary>
        /// Helper for building URLs that carry the current section, so that
        /// login/register pages can return the user to the right place.
        /// </summary>
        protected string CurrentSection
        {
            get
            {
                string path = (Request.AppRelativeCurrentExecutionFilePath ?? "").ToLowerInvariant();
                if (path.Contains("/puio/")) return "puio";
                return "jobs";
            }
        }
    }
}


/*using System;
using System.Web.UI;

namespace PSCIMS
{
    public partial class Site : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.IsAuthenticated)
            {
                LoginName1.Visible = true;
            }
        }

        protected string IsActive(string pageName)
        {
            string current = System.IO.Path.GetFileNameWithoutExtension(
                Request.AppRelativeCurrentExecutionFilePath);
            return string.Equals(current, pageName, StringComparison.OrdinalIgnoreCase)
                ? "active" : "";
        }
    }
}*/