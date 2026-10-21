using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using PSCIMS.Shared;

namespace PSCIMS.jobs
{
    public partial class ApplicantDashboard : System.Web.UI.Page
    {
        private int ApplicantId;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Request.IsAuthenticated || Session["ApplicantId"] == null)
            {
                Response.Redirect("~/jobs/loginPage.aspx");
                return;
            }

            ApplicantId = (int)Session["ApplicantId"];
            litName.Text = InputSanitizer.Enc(ApplicantDal.GetFirstName(ApplicantId));

            if (!IsPostBack) Bind();
        }

        private void Bind()
        {
            var rows = new List<object>();

            using (var conn = new SqlConnection(Config.ConnectionString))
            {
                conn.Open();
                SessionContext.Apply(conn);

                using (var cmd = new SqlCommand(
                    "SELECT a.Reference, a.Title, a.Ministry, ap.SubmittedAt, ap.Status " +
                    "FROM dbo.Applications ap " +
                    "INNER JOIN dbo.Adverts a ON a.Id = ap.AdvertId " +
                    "WHERE ap.ApplicantId = @Id " +
                    "ORDER BY ap.SubmittedAt DESC", conn))
                {
                    cmd.Parameters.AddWithValue("@Id", ApplicantId);

                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            rows.Add(new
                            {
                                Reference   = r["Reference"].ToString(),
                                Title       = r["Title"].ToString(),
                                Ministry    = r["Ministry"].ToString(),
                                SubmittedAt = Convert.ToDateTime(r["SubmittedAt"]),
                                Status      = r["Status"].ToString()
                            });
                        }
                    }
                }
            }

            phEmpty.Visible = rows.Count == 0;
            phList.Visible  = rows.Count > 0;
            rptApplications.DataSource = rows;
            rptApplications.DataBind();
        }

        protected string StatusClass(object status)
        {
            string s = (status ?? "").ToString().ToLowerInvariant();
            switch (s)
            {
                case "hired":        return "badge--success";
                case "shortlisted":  return "badge--info";
                case "rejected":     return "badge--danger";
                case "under_review": return "badge--open";
                default:             return "badge--info";
            }
        }
    }
}