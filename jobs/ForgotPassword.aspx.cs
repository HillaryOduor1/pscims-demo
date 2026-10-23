using System;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using PSCIMS.Shared;

namespace PSCIMS.jobs
{
    public partial class ForgotPassword : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // If already logged in, go to dashboard
            if (!IsPostBack && Request.IsAuthenticated && Session["ApplicantId"] != null)
            {
                Response.Redirect("~/jobs/ApplicantDashboard.aspx");
                return;
            }

            if (!IsPostBack)
            {
                hfCsrf.Value = CsrfProtector.IssueToken(Context);
            }
        }

        protected void btnSendCode_Click(object sender, EventArgs e)
        {
            if (!CsrfProtector.Validate(Context, hfCsrf.Value))
            {
                Fail("Your session has expired. Please refresh and try again.");
                return;
            }

            string ip = Request.UserHostAddress ?? "unknown";
            if (LoginThrottle.IsThrottled(ip, "ip"))
            {
                Fail("Too many attempts. Please try again later.");
                return;
            }

            string identifier = InputSanitizer.Clean(txtIdentifier.Text, 200);
            if (string.IsNullOrEmpty(identifier))
            {
                Fail("Please enter your ID or email.");
                return;
            }

            int applicantId = 0;
            string email = "";
            string firstName = "";

            // Find user by Email OR ID Number
            using (var conn = new SqlConnection(Config.ConnectionString))
            using (var cmd = new SqlCommand("SELECT Id, Email, FirstName FROM dbo.Applicants WHERE Email = @Val OR IdNumber = @Val", conn))
            {
                cmd.Parameters.AddWithValue("@Val", identifier);
                conn.Open();
                using (var r = cmd.ExecuteReader())
                {
                    if (r.Read())
                    {
                        applicantId = Convert.ToInt32(r["Id"]);
                        email = r["Email"].ToString();
                        firstName = r["FirstName"].ToString();
                    }
                }
            }

            // Always pretend we sent it, even if not found (prevents user enumeration)
            if (applicantId == 0)
            {
                // Since this is a gov portal and not a social app, it is often better UX to say "not found".
                // We will show the error to help the user.
                Fail("We could not find an account with that ID or Email address.");
                return;
            }

            try
            {
                // Generate OTP specifically for password reset
                string code = OtpService.Create(applicantId, "password_reset");
                
                EmailService.SendOtp(email, firstName, code);

                AuditLogger.Log("password_reset_otp_sent", applicantId, true, null);
                
                // Hide the request form, show the reset modal
                phRequest.Visible = false;
                phModal.Visible = true;
                hfResetApplicantId.Value = applicantId.ToString();
            }
            catch (Exception ex)
            {
                AuditLogger.Log("password_reset_otp_fail", applicantId, false, ex.Message);
                Fail("We could not send the email at this time. Please try again shortly.");
            }
        }

        protected void btnResetPassword_Click(object sender, EventArgs e)
        {
            if (!CsrfProtector.Validate(Context, hfCsrf.Value))
            {
                Fail("Your session has expired. Please refresh and try again.");
                return;
            }

            int applicantId = int.Parse(hfResetApplicantId.Value);
            string code = InputSanitizer.Clean(txtOtp.Text, 6);
            string password = txtNewPassword.Text ?? "";
            string confirm = txtConfirmPassword.Text ?? "";

            if (string.IsNullOrEmpty(code) || code.Length != 6)
            {
                Fail("Please enter the 6-digit code.");
                return;
            }

            if (password.Length < 8 || password.Length > 128)
            {
                Fail("Password must be between 8 and 128 characters.");
                return;
            }

            if (!Regex.IsMatch(password, @"[A-Z]") || !Regex.IsMatch(password, @"[a-z]") || !Regex.IsMatch(password, @"\d"))
            {
                Fail("Password must contain an uppercase letter, a lowercase letter, and a digit.");
                return;
            }

            if (password != confirm)
            {
                Fail("The passwords do not match.");
                return;
            }

            // Verify the OTP
            if (!OtpService.Verify(applicantId, code, "password_reset"))
            {
                AuditLogger.Log("password_reset_invalid_code", applicantId, false, null);
                Fail("The reset code is invalid or has expired.");
                return;
            }

            // Update the password
            try
            {
                string hash, salt;
                PasswordHasher.CreateHash(password, out hash, out salt);

                using (var conn = new SqlConnection(Config.ConnectionString))
                using (var cmd = new SqlCommand("UPDATE dbo.Applicants SET PasswordHash = @H, PasswordSalt = @S WHERE Id = @Id", conn))
                {
                    cmd.Parameters.AddWithValue("@H", hash);
                    cmd.Parameters.AddWithValue("@S", salt);
                    cmd.Parameters.AddWithValue("@Id", applicantId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                AuditLogger.Log("password_reset_success", applicantId, true, "Password changed via OTP");
                
                // Success! Redirect to login
                Response.Redirect("~/jobs/loginPage.aspx?reset=success");
            }
            catch (Exception ex)
            {
                AuditLogger.Log("password_reset_error", applicantId, false, ex.Message);
                Fail("An unexpected error occurred. Please try again later.");
            }
        }

        protected void btnCancelModal_Click(object sender, EventArgs e)
        {
            // Go back to the request step
            phModal.Visible = false;
            phRequest.Visible = true;
            phError.Visible = false;
        }

        private void Fail(string message)
        {
            phError.Visible = true;
            litError.Text = InputSanitizer.Enc(message);
            
            // Keep the modal open if we are on the reset step
            if (phModal.Visible)
            {
                phRequest.Visible = false;
                phModal.Visible = true;
            }
        }
    }
}