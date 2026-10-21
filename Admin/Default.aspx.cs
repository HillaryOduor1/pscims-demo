using System;

namespace PSCIMS.Admin
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Request.IsAuthenticated || Session["ApplicantRole"] == null || Session["ApplicantRole"].ToString().ToLower() != "admin")
            {
                Response.Redirect("~/jobs/LoginPage.aspx");
                return;
            }
        }
    }
}