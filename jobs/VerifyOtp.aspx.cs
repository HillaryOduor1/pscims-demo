using System;
using System.Web.Security;
using PSCIMS.Shared;

namespace PSCIMS.jobs
{
    public partial class VerifyOtp : System.Web.UI.Page
    {
        private int PendingApplicantId
        {
            get
            {
                var v = Session == null ? null : Session["PendingVerifyApplicantId"];
                return v == null ? 0 : (int)v;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && Request.IsAuthenticated && Session != null && Session["ApplicantId"] != null)
            {
                Response.Redirect("~/jobs/ApplicantDashboard.aspx");
                return;
            }

            if (PendingApplicantId == 0)
            {
                Response.Redirect("~/jobs/RegisterProfile.aspx");
                return;
            }

            if (!IsPostBack)
            {
                hfCsrf.Value = CsrfProtector.IssueToken(Context);
                litEmail.Text = InputSanitizer.Enc((string)Session["PendingVerifyEmail"] ?? "");
            }
        }

        protected void btnVerify_Click(object sender, EventArgs e)
        {
            int applicantId = PendingApplicantId;
            string code = InputSanitizer.Clean(txtCode.Text, 6);

            // DIAGNOSTIC LOG: Tells us exactly what the server sees
            AuditLogger.Log("otp_verify_click", applicantId, true, 
                "CodeEntered=" + code + "; SessionAppId=" + applicantId);

            if (applicantId == 0)
            {
                AuditLogger.Log("otp_verify_fail", 0, false, "Session lost. PendingApplicantId is 0.");
                Response.Redirect("~/jobs/RegisterProfile.aspx");
                return;
            }

            /* 
            if (!CsrfProtector.Validate(Context, hfCsrf.Value))
            {
                AuditLogger.Log("otp_csrf_fail", applicantId, false, "CSRF token mismatch");
                Fail("Your session has expired. Please refresh and try again.");
                return;
            }
            */

            if (string.IsNullOrEmpty(code) || code.Length != 6)
            {
                Fail("Please enter the 6-digit code.");
                return;
            }

            try
            {
                bool verified = OtpService.Verify(applicantId, code, "email_verify");
                
                if (verified)
                {
                    AuditLogger.Log("otp_verify_success", applicantId, true, "OTP successfully validated");
                    
                    ApplicantDal.MarkEmailVerified(applicantId);
                    Session.Remove("PendingVerifyApplicantId");
                    Session.Remove("PendingVerifyEmail");

                    string firstName = ApplicantDal.GetFirstName(applicantId);
                    string email = ApplicantDal.GetEmail(applicantId);

                    FormsAuthentication.SetAuthCookie(applicantId.ToString(), false);
                    Session["ApplicantId"]          = applicantId;
                    Session["ApplicantRole"]        = "applicant";
                    Session["ApplicantIdNumber"]    = "";
                    Session["ApplicantFirstName"]   = firstName;
                    Session["ApplicantDisplayName"] = string.IsNullOrEmpty(firstName) ? email : firstName;

                    ApplicantDal.RecordLogin(applicantId);
                    AuditLogger.Log("login_ok", applicantId, true, "Auto-login after email verification");

                    if (Session["PendingApplyJobId"] != null || Session["PendingApplyInternshipId"] != null)
                    {
                        Response.Redirect("~/jobs/Application.aspx");
                    }
                    else
                    {
                        Response.Redirect("~/jobs/ApplicantDashboard.aspx");
                    }
                }
                else
                {
                    AuditLogger.Log("otp_verify_fail", applicantId, false, "OtpService returned false (Invalid, expired, or consumed)");
                    Fail("Invalid or expired code.");
                }
            }
            catch (System.Threading.ThreadAbortException)
            {
                throw;
            }
            catch (Exception ex)
            {
                string fullMsg = ex.GetType().Name + ": " + ex.Message;
                if (ex.InnerException != null)
                    fullMsg += " || INNER: " + ex.InnerException.Message;
                AuditLogger.Log("otp_verify_error", applicantId, false, fullMsg);
                Fail("An unexpected error occurred. Please try again.");
            }
        }

        protected void lnkResend_Click(object sender, EventArgs e)
        {
            int applicantId = PendingApplicantId;
            if (applicantId == 0) return;

            AuditLogger.Log("otp_resend_attempt", applicantId, true, null);

            try
            {
                string code = OtpService.Create(applicantId, "email_verify");
                string email = ApplicantDal.GetEmail(applicantId);
                string name = ApplicantDal.GetFirstName(applicantId);

                EmailService.SendOtp(email, name, code);
                AuditLogger.Log("otp_resent", applicantId, true, "Email sent to " + email);

                phError.Visible = true;
                alertBox.Attributes["class"] = "alert alert--success";
                litError.Text = InputSanitizer.Enc("A new code has been sent. Check your inbox.");
            }
            catch (Exception ex)
            {
                AuditLogger.Log("otp_resend_fail", applicantId, false, ex.Message);
                Fail("Could not resend the code. Please try again shortly.");
            }
        }

        private void Fail(string message)
        {
            phError.Visible = true;
            alertBox.Attributes["class"] = "alert alert--danger";
            litError.Text = InputSanitizer.Enc(message);
        }
    }
}

/*using System;
using System.Web.Security;
using PSCIMS.Shared;

namespace PSCIMS.jobs
{
    public partial class VerifyOtp : System.Web.UI.Page
    {
        private int PendingApplicantId
        {
            get
            {
                var v = Session == null ? null : Session["PendingVerifyApplicantId"];
                return v == null ? 0 : (int)v;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && Request.IsAuthenticated && Session != null && Session["ApplicantId"] != null)
            {
                Response.Redirect("~/jobs/ApplicantDashboard.aspx");
                return;
            }

            if (PendingApplicantId == 0)
            {
                Response.Redirect("~/jobs/RegisterProfile.aspx");
                return;
            }

            if (!IsPostBack)
            {
                hfCsrf.Value = CsrfProtector.IssueToken(Context);
                litEmail.Text = InputSanitizer.Enc((string)Session["PendingVerifyEmail"] ?? "");
            }
        }

        protected void btnVerify_Click(object sender, EventArgs e)
        {
            AuditLogger.Log("otp_verify_attempt", PendingApplicantId, true, "Verify button clicked");

            if (!CsrfProtector.Validate(Context, hfCsrf.Value))
            {
                Fail("Your session has expired. Please refresh and try again.");
                return;
            }

            string code = InputSanitizer.Clean(txtCode.Text, 6);
            if (string.IsNullOrEmpty(code) || code.Length != 6)
            {
                Fail("Please enter the 6-digit code.");
                return;
            }

            int applicantId = PendingApplicantId;

            try
            {
                if (OtpService.Verify(applicantId, code, "email_verify"))
                {
                    ApplicantDal.MarkEmailVerified(applicantId);
                    AuditLogger.Log("email_verified", applicantId, true, null);

                    Session.Remove("PendingVerifyApplicantId");
                    Session.Remove("PendingVerifyEmail");

                    // ---- Issue auth cookie and populate session ----
                    string firstName = ApplicantDal.GetFirstName(applicantId);
                    string email = ApplicantDal.GetEmail(applicantId);

                    FormsAuthentication.SetAuthCookie(applicantId.ToString(), false);
                    Session["ApplicantId"]          = applicantId;
                    Session["ApplicantRole"]        = "applicant";
                    Session["ApplicantIdNumber"]    = "";
                    Session["ApplicantFirstName"]   = firstName;
                    Session["ApplicantDisplayName"] = string.IsNullOrEmpty(firstName) ? email : firstName;

                    ApplicantDal.RecordLogin(applicantId);
                    AuditLogger.Log("login_ok", applicantId, true, "Auto-login after email verification");

                    // --- NEW: Check for Application Intent ---
                    if (Session["PendingApplyJobId"] != null || Session["PendingApplyInternshipId"] != null)
                    {
                        Response.Redirect("~/jobs/Application.aspx");
                    }
                    else
                    {
                        Response.Redirect("~/jobs/ApplicantDashboard.aspx");
                    }
                }
                else
                {
                    AuditLogger.Log("otp_verify_fail", applicantId, false, "Invalid or expired code");
                    Fail("Invalid or expired code.");
                }
            }
            catch (System.Threading.ThreadAbortException)
            {
                throw;
            }
            catch (Exception ex)
            {
                string fullMsg = ex.GetType().Name + ": " + ex.Message;
                if (ex.InnerException != null)
                    fullMsg += " || INNER: " + ex.InnerException.Message;
                AuditLogger.Log("otp_verify_error", applicantId, false, fullMsg);
                Fail("An unexpected error occurred. Please try again.");
            }
        }

        protected void lnkResend_Click(object sender, EventArgs e)
        {
            int applicantId = PendingApplicantId;
            if (applicantId == 0) return;

            AuditLogger.Log("otp_resend_attempt", applicantId, true, null);

            try
            {
                string code = OtpService.Create(applicantId, "email_verify");
                string email = ApplicantDal.GetEmail(applicantId);
                string name = ApplicantDal.GetFirstName(applicantId);

                EmailService.SendOtp(email, name, code);
                AuditLogger.Log("otp_resent", applicantId, true, "Email sent to " + email);

                phError.Visible = true;
                alertBox.Attributes["class"] = "alert alert--success";
                litError.Text = InputSanitizer.Enc("A new code has been sent. Check your inbox.");
            }
            catch (Exception ex)
            {
                AuditLogger.Log("otp_resend_fail", applicantId, false, ex.Message);
                Fail("Could not resend the code. Please try again shortly.");
            }
        }

        private void Fail(string message)
        {
            phError.Visible = true;
            alertBox.Attributes["class"] = "alert alert--danger";
            litError.Text = InputSanitizer.Enc(message);
        }
    }
}*/