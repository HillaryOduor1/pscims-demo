using System;
using System.Data;
using System.Data.SqlClient;
using PSCIMS.Shared;

namespace PSCIMS.Admin
{
    public partial class ViewApplications : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Request.IsAuthenticated || Session["ApplicantRole"] == null || Session["ApplicantRole"].ToString().ToLower() != "admin")
            {
                Response.Redirect("~/jobs/LoginPage.aspx");
                return;
            }

            if (!IsPostBack)
            {
                BindGrid();
            }
        }

        private void BindGrid()
        {
            string sql = @"
                SELECT 
                    app.Id AS ApplicationId, 
                    adv.Title AS AdvertTitle, 
                    a.FirstName + ' ' + a.Surname AS ApplicantName, 
                    a.Email AS ApplicantEmail,
                    app.SubmittedAt AS SubmissionDate
                FROM dbo.Applications app
                INNER JOIN dbo.Applicants a ON app.ApplicantId = a.Id
                INNER JOIN dbo.Adverts adv ON app.AdvertId = adv.Id
                ORDER BY app.SubmittedAt DESC";

            using (var conn = new SqlConnection(Config.ConnectionString))
            using (var cmd = new SqlCommand(sql, conn))
            {
                using (var da = new SqlDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    gvApps.DataSource = dt;
                    gvApps.DataBind();
                }
            }
        }
    }
}