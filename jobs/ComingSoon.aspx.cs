using System;
using System.Web;

namespace PSCIMS.jobs
{
    public partial class ComingSoon : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string requested = Request.QueryString["page"];
            string friendly = FriendlyName(requested);

            if (!string.IsNullOrEmpty(friendly))
            {
                litTitle.Text = HttpUtility.HtmlEncode(friendly) + " — coming soon";
                litMessage.Text =
                    "We're putting the finishing touches on the " +
                    "<strong>" + HttpUtility.HtmlEncode(friendly) + "</strong> " +
                    "section. It will be available here very shortly.";
            }
            else
            {
                litTitle.Text = "This page is coming soon";
                litMessage.Text =
                    "The page you requested is not yet available. " +
                    "We're working on it and will publish it soon.";
            }
        }

        private static string FriendlyName(string requested)
        {
            if (string.IsNullOrWhiteSpace(requested)) return "";

            string name = requested;
            int q = name.IndexOf('?');
            if (q >= 0) name = name.Substring(0, q);

            // Strip the .aspx extension
            name = System.IO.Path.GetFileNameWithoutExtension(name);

            // Strip a trailing ".partial" if present
            if (name.EndsWith(".partial", StringComparison.OrdinalIgnoreCase))
            {
                name = name.Substring(0, name.Length - ".partial".Length);
            }

            if (string.IsNullOrEmpty(name)) return "";

            switch (name.ToLowerInvariant())
            {
                case "activejobsadverts":                 return "Advertised Jobs";
                case "activeadvertsinternsinternshipext": return "Internships";
                case "advertstatusglobal":                return "Application Status";
                case "elpext":                            return "Emerging Leaders Fellowship";
                case "applicantdashboard":                return "Your Applications";
                case "forgotpassword":                    return "Password Recovery";
                case "application":                       return "Job Application";
                case "registersuccess":                   return "Registration";
                default:
                    // Fallback: humanise "SomePageName" -> "Some Page Name"
                    var sb = new System.Text.StringBuilder();
                    foreach (char c in name)
                    {
                        if (char.IsUpper(c) && sb.Length > 0) sb.Append(' ');
                        sb.Append(c);
                    }
                    return sb.ToString().Trim();
            }
        }
    }
}