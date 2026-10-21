using System;

namespace PSCIMS.jobs
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // If authenticated, send the user to their dashboard.
            if (Request.IsAuthenticated && Session != null && Session["ApplicantId"] != null)
            {
                Response.Redirect("~/jobs/ApplicantDashboard.aspx");
            }
        }
    }
}