using System;
using PSCIMS.Shared;

namespace PSCIMS.jobs
{
    public partial class RegisterProfile : System.Web.UI.Page
    {
        private string Section
        {
            get
            {
                string from = (hfFrom.Value ?? "").Trim().ToLowerInvariant();
                if (from == "puio") return "puio";
                string q = (Request.QueryString["from"] ?? "").Trim().ToLowerInvariant();
                if (q == "puio") return "puio";
                return "jobs";
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // TEMP DIAGNOSTIC
    try
    {
        System.IO.File.AppendAllText(@"C:\temp1\page_load_ran.log",
            DateTime.Now.ToString() + " Page_Load fired\r\n");
    }
    catch { }
            // If already logged in, go to dashboard.
            if (!IsPostBack && Request.IsAuthenticated
                && Session != null && Session["ApplicantId"] != null)
            {
                Response.Redirect("~/jobs/ApplicantDashboard.aspx");
                return;
            }

            if (!IsPostBack)
            {
                hfCsrf.Value = CsrfProtector.IssueToken(Context);

                Random rnd = new Random();
                int s1 = rnd.Next(1, 10);
                int s2 = rnd.Next(1, 10);

                litStamp1.Text = s1.ToString();
                litStamp2.Text = s2.ToString();

                ViewState["ExpectedStampSum"] = (s1 + s2).ToString();

                string qFrom = (Request.QueryString["from"] ?? "").Trim().ToLowerInvariant();
                if (qFrom == "puio") hfFrom.Value = "puio";

                string loginUrl = Section == "puio"
                    ? ResolveUrl("~/jobs/loginPage.aspx?from=puio")
                    : ResolveUrl("~/jobs/loginPage.aspx");

                lnkSignIn.NavigateUrl = loginUrl;
                lnkSignIn.Attributes["data-nav"] = "data-nav";

                lnkSuccessSignIn.NavigateUrl = loginUrl;
                lnkSuccessSignIn.Attributes["data-nav"] = "data-nav";
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            AuditLogger.Log("register_attempt_start", null, true, "Method entered");

            if (!CsrfProtector.Validate(Context, hfCsrf.Value))
            {
                AuditLogger.Log("register_csrf_fail", null, false, null);
                Fail("Your session has expired. Please refresh the page and try again.");
                return;
            }

            string ip = Request.UserHostAddress ?? "unknown";
            try
            {
                if (LoginThrottle.IsThrottled(ip, "ip"))
                {
                    AuditLogger.Log("register_throttled", null, false, "IP " + ip);
                    Fail("Too many attempts. Please try again in 15 minutes.");
                    return;
                }
            }
            catch (Exception ex)
            {
                AuditLogger.Log("register_throttle_check_fail", null, false, ex.Message);
            }

            string idNum       = InputSanitizer.Clean(txtIdNum.Text, 20);
            string surname     = InputSanitizer.Clean(txtSurname.Text, 100);
            string firstName   = InputSanitizer.Clean(txtFirstName.Text, 100);
            string otherNames  = InputSanitizer.Clean(txtOtherNames.Text, 150);
            string email       = InputSanitizer.Clean(txtEmail.Text, 200).ToLowerInvariant();
            string emailConf   = InputSanitizer.Clean(txtEmailConfirm.Text, 200).ToLowerInvariant();
            string password    = txtPassword.Text ?? "";
            string passwordCf  = txtPasswordConfirm.Text ?? "";
            string stampAnswer = InputSanitizer.Clean(txtResult.Text, 5);

            if (string.IsNullOrEmpty(idNum) || string.IsNullOrEmpty(surname) ||
                string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(email) ||
                string.IsNullOrEmpty(password) || string.IsNullOrEmpty(stampAnswer))
            {
                Fail("Please complete all required fields.");
                return;
            }

            if (!InputSanitizer.IsValidId(idNum))
            {
                Fail("ID / Passport Number must be 7-10 alphanumeric characters.");
                return;
            }

            if (!InputSanitizer.IsValidName(surname) || !InputSanitizer.IsValidName(firstName))
            {
                Fail("Names must contain only letters, spaces, hyphens, apostrophes, or periods.");
                return;
            }

            if (!string.IsNullOrEmpty(otherNames) && !InputSanitizer.IsValidName(otherNames))
            {
                Fail("Other names must contain only letters, spaces, hyphens, apostrophes, or periods.");
                return;
            }

            if (!InputSanitizer.IsValidEmail(email))
            {
                Fail("Please enter a valid email address.");
                return;
            }

            if (!string.Equals(email, emailConf, StringComparison.Ordinal))
            {
                Fail("The email addresses do not match.");
                return;
            }

            if (password.Length < 8 || password.Length > 128)
            {
                Fail("Password must be between 8 and 128 characters.");
                return;
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(password, @"[A-Z]") ||
                !System.Text.RegularExpressions.Regex.IsMatch(password, @"[a-z]") ||
                !System.Text.RegularExpressions.Regex.IsMatch(password, @"\d"))
            {
                Fail("Password must contain an uppercase letter, a lowercase letter, and a digit.");
                return;
            }

            if (!string.Equals(password, passwordCf, StringComparison.Ordinal))
            {
                Fail("The passwords do not match.");
                return;
            }

            string expected = ViewState["ExpectedStampSum"] as string;
            if (expected == null || !string.Equals(stampAnswer, expected, StringComparison.Ordinal))
            {
                Fail("Security check failed. Please try again.");
                return;
            }

            try
            {
                string target = "unknown";
                int connLen = 0;

                try
                {
                    target = Config.WhichDatabase();
                    var connStr = Config.ConnectionString;
                    connLen = connStr == null ? 0 : connStr.Length;
                }
                catch (Exception cfgEx)
                {
                    AuditLogger.Log("register_config_fail", null, false,
                        cfgEx.GetType().Name + ": " + cfgEx.Message);
                    Fail("Configuration error: " + cfgEx.Message);
                    return;
                }

                AuditLogger.Log("register_db_start", null, true,
                    "Target=" + target + " ConnStrLength=" + connLen);

                bool exists;
                try
                {
                    exists = ApplicantDal.Exists(idNum, email);
                }
                catch (Exception ex)
                {
                    AuditLogger.Log("register_exists_check_fail", null, false,
                        ex.GetType().Name + ": " + ex.Message +
                        (ex.InnerException != null ? " || INNER: " + ex.InnerException.Message : ""));
                    Fail("Could not check for duplicate account: " + ex.Message);
                    return;
                }

                if (exists)
                {
                    AuditLogger.Log("register_duplicate", null, false,
                        "ID=" + idNum + " Email=" + email);
                    Fail("An account with that ID or email already exists.");
                    return;
                }

                int newId;
                try
                {
                    newId = ApplicantDal.Register(idNum, surname, firstName,
                        string.IsNullOrEmpty(otherNames) ? null : otherNames,
                        email, password);

                    AuditLogger.Log("register_inserted", newId, true, "New applicant Id=" + newId);
                }
                catch (Exception ex)
                {
                    AuditLogger.Log("register_insert_fail", null, false,
                        ex.GetType().Name + ": " + ex.Message +
                        (ex.InnerException != null ? " || INNER: " + ex.InnerException.Message : ""));
                    Fail("Could not create account: " + ex.Message);
                    return;
                }

                string code;
                try
                {
                    code = OtpService.Create(newId, "email_verify");
                    AuditLogger.Log("otp_created", newId, true,
                        "OTP length=" + (code == null ? 0 : code.Length));
                }
                catch (Exception otpEx)
                {
                    AuditLogger.Log("otp_create_fail", newId, false,
                        otpEx.GetType().Name + ": " + otpEx.Message +
                        (otpEx.InnerException != null ? " || INNER: " + otpEx.InnerException.Message : ""));

                    Session["PendingVerifyApplicantId"] = newId;
                    Session["PendingVerifyEmail"] = email;
                    Response.Redirect("~/jobs/VerifyOtp.aspx");
                    return;
                }

                try
                {
                    EmailService.SendOtp(email, firstName, code);
                    AuditLogger.Log("otp_sent", newId, true, "Email sent to " + email);
                }
                catch (Exception mailEx)
                {
                    AuditLogger.Log("otp_send_fail", newId, false,
                        mailEx.GetType().Name + ": " + mailEx.Message +
                        (mailEx.InnerException != null ? " || INNER: " + mailEx.InnerException.Message : ""));
                }

                Session["PendingVerifyApplicantId"] = newId;
                Session["PendingVerifyEmail"] = email;
                Response.Redirect("~/jobs/VerifyOtp.aspx");
            }
            catch (System.Threading.ThreadAbortException)
            {
                throw;
            }
            catch (Exception ex)
            {
                string fullMsg = ex.GetType().Name + ": " + ex.Message;
                if (ex.InnerException != null)
                {
                    fullMsg += " || INNER: " + ex.InnerException.GetType().Name + ": " + ex.InnerException.Message;
                }
                AuditLogger.Log("register_error", null, false, fullMsg);
                Fail("Registration failed: " + fullMsg);
            }
        }

        private void Fail(string message)
        {
            phError.Visible = true;
            phSuccess.Visible = false;
            litError.Text = InputSanitizer.Enc(message);
        }
    }
}

