using System;
using System.Data.SqlClient;
using PSCIMS.Shared;

namespace PSCIMS.jobs
{
    public partial class Application : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Request.IsAuthenticated || Session["ApplicantId"] == null)
            {
                Response.Redirect("~/jobs/loginPage.aspx");
                return;
            }

            // Check if they have a pending intent
            if (Session["PendingApplyJobId"] == null && Session["PendingApplyInternshipId"] == null)
            {
                Response.Redirect("~/jobs/ApplicantDashboard.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadAdvertDetails();
            }
        }

        private void LoadAdvertDetails()
        {
            string idStr = Session["PendingApplyJobId"] as string ?? Session["PendingApplyInternshipId"] as string;
            if (string.IsNullOrEmpty(idStr)) return;

            using (var conn = new SqlConnection(Config.ConnectionString))
            using (var cmd = new SqlCommand("SELECT Title, Ministry FROM dbo.Adverts WHERE Id = @Id", conn))
            {
                cmd.Parameters.AddWithValue("@Id", int.Parse(idStr));
                conn.Open();
                using (var r = cmd.ExecuteReader())
                {
                    if (r.Read())
                    {
                        litJobTitle.Text = InputSanitizer.Enc(r["Title"].ToString());
                        litJobMinistry.Text = InputSanitizer.Enc(r["Ministry"].ToString());
                    }
                    else
                    {
                        ShowError("The position you are trying to apply for is no longer available.");
                    }
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string idStr = Session["PendingApplyJobId"] as string ?? Session["PendingApplyInternshipId"] as string;
            int applicantId = (int)Session["ApplicantId"];

            if (string.IsNullOrEmpty(idStr)) return;

            try
            {
                using (var conn = new SqlConnection(Config.ConnectionString))
                using (var cmd = new SqlCommand(
                    "INSERT INTO dbo.Applications (ApplicantId, AdvertId, Status, SubmittedAt) " +
                    "VALUES (@AppId, @AdvId, 'submitted', SYSUTCDATETIME())", conn))
                {
                    cmd.Parameters.AddWithValue("@AppId", applicantId);
                    cmd.Parameters.AddWithValue("@AdvId", int.Parse(idStr));
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                // Clear the intent so they don't double apply by pressing back
                Session.Remove("PendingApplyJobId");
                Session.Remove("PendingApplyInternshipId");

                phForm.Visible = false;
                phSuccess.Visible = true;
            }
            catch (SqlException ex)
            {
                // 2627 is Unique constraint violation (already applied)
                if (ex.Number == 2627 || ex.Number == 2601)
                {
                    ShowError("You have already applied for this position.");
                }
                else
                {
                    ShowError("Error submitting application: " + ex.Message);
                }
            }
            catch (Exception ex)
            {
                ShowError("Error submitting application: " + ex.Message);
            }
        }

        private void ShowError(string msg)
        {
            phError.Visible = true;
            litError.Text = InputSanitizer.Enc(msg);
            phForm.Visible = false;
        }
    }
}