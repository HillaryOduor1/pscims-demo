using System;
using System.Web.Security;
using PSCIMS.Shared;

namespace PSCIMS.jobs
{
    public partial class loginPage : System.Web.UI.Page
    {
        private string Section
        {
            get
            {
                string from = (Request.QueryString["from"] ?? "").Trim().ToLowerInvariant();
                return from == "puio" ? "puio" : "jobs";
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // CRITICAL FIX: Ensure the login page is never cached
            Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.AppendHeader("Pragma", "no-cache");

            if (!IsPostBack && Request.IsAuthenticated && Session != null && Session["ApplicantId"] != null)
            {
                Response.Redirect("~/jobs/ApplicantDashboard.aspx");
                return;
            }

            if (!IsPostBack)
            {
                // --- Capture Application Intent ---
                string jobId = Request.QueryString["jobId"];
                string intId = Request.QueryString["internshipId"];
                
                if (!string.IsNullOrEmpty(jobId)) Session["PendingApplyJobId"] = jobId;
                if (!string.IsNullOrEmpty(intId)) Session["PendingApplyInternshipId"] = intId;

                hfCsrf.Value = CsrfProtector.IssueToken(Context);

                string regUrl = Section == "puio"
                    ? ResolveUrl("~/jobs/RegisterProfile.aspx?from=puio")
                    : ResolveUrl("~/jobs/RegisterProfile.aspx");
                
                if (!string.IsNullOrEmpty(jobId)) regUrl += "?jobId=" + jobId;
                else if (!string.IsNullOrEmpty(intId)) regUrl += "?internshipId=" + intId;

                lnkRegister.NavigateUrl = regUrl;
                lnkRegister.Attributes["data-nav"] = "data-nav";
                lnkForgot.NavigateUrl = ResolveUrl("~/jobs/ForgotPassword.aspx");
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            // ... (rest of the login logic remains the same) ...
            AuditLogger.Log("login_attempt_start", null, true, "Login button clicked, ID=" + (txtId.Text ?? ""));

            if (!CsrfProtector.Validate(Context, hfCsrf.Value))
            {
                AuditLogger.Log("login_csrf_fail", null, false, null);
                Fail("Your session has expired. Please refresh and try again.");
                return;
            }

            string id = InputSanitizer.Clean(txtId.Text, 20);
            string pwd = txtPassword.Text ?? "";
            string ip = Request.UserHostAddress ?? "unknown";

            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(pwd))
            {
                Fail("Please enter both your ID / Passport Number and password.");
                return;
            }

            if (!InputSanitizer.IsValidId(id))
            {
                AuditLogger.Log("login_bad_id_format", null, false, "IP " + ip);
                Fail("Invalid ID number or password.");
                return;
            }

            if (LoginThrottle.IsThrottled(ip, "ip") || LoginThrottle.IsThrottled(id, "id"))
            {
                AuditLogger.Log("login_throttled", null, false, "IP " + ip + " ID " + id);
                Fail("Too many failed attempts. Please try again in 15 minutes.");
                return;
            }

            try
            {
                var result = ApplicantDal.Validate(id, pwd);

                if (result == null)
                {
                    LoginThrottle.Record(ip, "ip", false);
                    LoginThrottle.Record(id, "id", false);
                    AuditLogger.Log("login_fail", null, false, "ID " + id);
                    Fail("Invalid ID number or password.");
                    return;
                }

                if (!result.EmailVerified)
                {
                    AuditLogger.Log("login_not_verified", result.Id, true, "Redirect to OTP page");
                    Session["PendingVerifyApplicantId"] = result.Id;
                    Session["PendingVerifyEmail"] = ApplicantDal.GetEmail(result.Id);
                    Response.Redirect("~/jobs/VerifyOtp.aspx");
                    return;
                }

                string firstName = ApplicantDal.GetFirstName(result.Id);

                FormsAuthentication.SetAuthCookie(result.Id.ToString(), false);
                Session["ApplicantId"]          = result.Id;
                Session["ApplicantRole"]        = result.Role;
                Session["ApplicantIdNumber"]    = id;
                Session["ApplicantFirstName"]   = firstName;
                Session["ApplicantDisplayName"] = string.IsNullOrEmpty(firstName) ? id : firstName;

                ApplicantDal.RecordLogin(result.Id);
                LoginThrottle.Record(ip, "ip", true);
                AuditLogger.Log("login_ok", result.Id, true, "Role: " + result.Role);

                if (Session["PendingApplyJobId"] != null || Session["PendingApplyInternshipId"] != null)
                {
                    Response.Redirect("~/jobs/Application.aspx");
                }
                else
                {
                    string target;
                    if (result.Role.ToLower() == "admin")
                        target = "~/Admin/Default.aspx";
                    else if (Section == "puio")
                        target = "~/puio/Default.aspx";
                    else
                        target = "~/jobs/ApplicantDashboard.aspx";
                    
                    Response.Redirect(target);
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
                AuditLogger.Log("login_error", null, false, fullMsg);
                Fail("An unexpected error occurred. Please try again.");
            }
        }

        private void Fail(string message)
        {
            phError.Visible = true;
            litError.Text = InputSanitizer.Enc(message);
        }
    }
}