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


/*using System;
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
            if (!IsPostBack)
            {
                // CSRF token
                hfCsrf.Value = CsrfProtector.IssueToken(Context);

                // Security check — generate two random numbers
                Random rnd = new Random();
                int s1 = rnd.Next(1, 10);
                int s2 = rnd.Next(1, 10);

                litStamp1.Text = s1.ToString();
                litStamp2.Text = s2.ToString();

                ViewState["ExpectedStampSum"] = (s1 + s2).ToString();

                // Return hint
                string qFrom = (Request.QueryString["from"] ?? "").Trim().ToLowerInvariant();
                if (qFrom == "puio") hfFrom.Value = "puio";

                // Set the "already have account" link
                string loginUrl = Section == "puio"
                    ? ResolveUrl("~/jobs/loginPage.aspx?from=puio")
                    : ResolveUrl("~/jobs/loginPage.aspx");

                lnkSignIn.NavigateUrl = loginUrl;
                lnkSignIn.Attributes["data-nav"] = "data-nav";

                // Success panel link
                lnkSuccessSignIn.NavigateUrl = loginUrl;
                lnkSuccessSignIn.Attributes["data-nav"] = "data-nav";
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            AuditLogger.Log("register_attempt_start", null, true, "Method entered");

            // ---- CSRF ----
            if (!CsrfProtector.Validate(Context, hfCsrf.Value))
            {
                AuditLogger.Log("register_csrf_fail", null, false, null);
                Fail("Your session has expired. Please refresh the page and try again.");
                return;
            }

            // ---- Rate limit by IP ----
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
                // Continue — don't block registration if throttle check fails
            }

            // ---- Sanitize inputs ----
            string idNum       = InputSanitizer.Clean(txtIdNum.Text, 20);
            string surname     = InputSanitizer.Clean(txtSurname.Text, 100);
            string firstName   = InputSanitizer.Clean(txtFirstName.Text, 100);
            string otherNames  = InputSanitizer.Clean(txtOtherNames.Text, 150);
            string email       = InputSanitizer.Clean(txtEmail.Text, 200).ToLowerInvariant();
            string emailConf   = InputSanitizer.Clean(txtEmailConfirm.Text, 200).ToLowerInvariant();
            string password    = txtPassword.Text ?? "";
            string passwordCf  = txtPasswordConfirm.Text ?? "";
            string stampAnswer = InputSanitizer.Clean(txtResult.Text, 5);

            // ---- Validate ----
            if (string.IsNullOrEmpty(idNum) || string.IsNullOrEmpty(surname) ||
                string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(email) ||
                string.IsNullOrEmpty(password) || string.IsNullOrEmpty(stampAnswer))
            {
                AuditLogger.Log("register_validation_fail", null, false, "Missing required fields");
                Fail("Please complete all required fields.");
                return;
            }

            if (!InputSanitizer.IsValidId(idNum))
            {
                AuditLogger.Log("register_validation_fail", null, false, "Invalid ID format: " + idNum);
                Fail("ID / Passport Number must be 7-10 alphanumeric characters.");
                return;
            }

            if (!InputSanitizer.IsValidName(surname) || !InputSanitizer.IsValidName(firstName))
            {
                AuditLogger.Log("register_validation_fail", null, false, "Invalid name format");
                Fail("Names must contain only letters, spaces, hyphens, apostrophes, or periods.");
                return;
            }

            if (!string.IsNullOrEmpty(otherNames) && !InputSanitizer.IsValidName(otherNames))
            {
                AuditLogger.Log("register_validation_fail", null, false, "Invalid other names");
                Fail("Other names must contain only letters, spaces, hyphens, apostrophes, or periods.");
                return;
            }

            if (!InputSanitizer.IsValidEmail(email))
            {
                AuditLogger.Log("register_validation_fail", null, false, "Invalid email: " + email);
                Fail("Please enter a valid email address.");
                return;
            }

            if (!string.Equals(email, emailConf, StringComparison.Ordinal))
            {
                AuditLogger.Log("register_validation_fail", null, false, "Email mismatch");
                Fail("The email addresses do not match.");
                return;
            }

            if (password.Length < 8 || password.Length > 128)
            {
                AuditLogger.Log("register_validation_fail", null, false, "Password length: " + password.Length);
                Fail("Password must be between 8 and 128 characters.");
                return;
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(password, @"[A-Z]") ||
                !System.Text.RegularExpressions.Regex.IsMatch(password, @"[a-z]") ||
                !System.Text.RegularExpressions.Regex.IsMatch(password, @"\d"))
            {
                AuditLogger.Log("register_validation_fail", null, false, "Password complexity");
                Fail("Password must contain an uppercase letter, a lowercase letter, and a digit.");
                return;
            }

            if (!string.Equals(password, passwordCf, StringComparison.Ordinal))
            {
                AuditLogger.Log("register_validation_fail", null, false, "Password mismatch");
                Fail("The passwords do not match.");
                return;
            }

            string expected = ViewState["ExpectedStampSum"] as string;
            if (expected == null || !string.Equals(stampAnswer, expected, StringComparison.Ordinal))
            {
                AuditLogger.Log("register_validation_fail", null, false,
                    "Stamp answer=" + stampAnswer + " expected=" + (expected ?? "null"));
                Fail("Security check failed. Please try again.");
                return;
            }

            // =================================================================
            // DB operations — every step logged, every exception captured
            // =================================================================
            try
            {
                string target = "unknown";
                string connStr = "";
                int connLen = 0;

                try
                {
                    target = Config.WhichDatabase();
                    connStr = Config.ConnectionString;
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

                // ---- Uniqueness check ----
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

                // ---- Insert applicant ----
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

                // ---- Create OTP ----
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

                    // The applicant was created — don't discard them.
                    // Redirect to VerifyOtp anyway; user can use "Resend code".
                    Session["PendingVerifyApplicantId"] = newId;
                    Session["PendingVerifyEmail"] = email;
                    Response.Redirect("~/jobs/VerifyOtp.aspx");
                    return;
                }

                // ---- Send OTP email (best-effort) ----
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
                    // Continue — user can still verify via resend, or admin can read OTP from DB
                }

                // ---- Success: redirect to OTP verification ----
                Session["PendingVerifyApplicantId"] = newId;
                Session["PendingVerifyEmail"] = email;
                Response.Redirect("~/jobs/VerifyOtp.aspx");
            }
            catch (System.Threading.ThreadAbortException)
            {
                // ASP.NET uses this to abort the thread during Response.Redirect.
                // It is not a real error — let it propagate.
                throw;
            }
            catch (Exception ex)
            {
                // Catch-all for anything we missed
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
}*/

/*using System;
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
                string connStr = "";
                int connLen = 0;

                try
                {
                    target = Config.WhichDatabase();
                    connStr = Config.ConnectionString;
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
}*/


/*<%@ Page Title="Register" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="RegisterProfile.aspx.cs" Inherits="PSCIMS.jobs.RegisterProfile" %>

<asp:Content ID="PageTitleC" ContentPlaceHolderID="PageTitle" runat="server">Register</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">

    <div style="max-width:720px;margin:0 auto;">

        <div class="mb-6">
            <h2 class="page-title">Create your applicant profile</h2>
            <p class="page-sub">
                Register once with your ID or Passport Number. You can then apply to any
                advertised position â€” public service, public universities, or independent offices.
            </p>
        </div>

        <asp:PlaceHolder ID="phError" runat="server" Visible="false">
            <div class="alert alert--danger" role="alert">
                <div class="alert__title">Could not complete registration</div>
                <asp:Literal ID="litError" runat="server" />
            </div>
        </asp:PlaceHolder>

        <asp:PlaceHolder ID="phSuccess" runat="server" Visible="false">
            <div class="alert alert--success" role="alert">
                <div class="alert__title">Registration successful</div>
                Your applicant profile has been created. You can now
                <asp:HyperLink ID="lnkSuccessSignIn" runat="server" Text="sign in" />
                to apply for advertised positions.
            </div>
        </asp:PlaceHolder>

        <div class="card" style="padding:28px;">

            <h3 class="card-title" style="margin-bottom:4px;">Personal information</h3>
            <p class="card-sub">As they appear on your national ID or passport.</p>

            <div class="form-group">
                <label class="form-label" for="<%= txtIdNum.ClientID %>">
                    ID / Passport Number <span class="req" aria-hidden="true">*</span>
                </label>
                <asp:TextBox ID="txtIdNum" runat="server" CssClass="form-control"
                             MaxLength="20" autocomplete="off" inputmode="numeric"
                             required="required" />
                <div class="form-hint">Enter digits only, no spaces or slashes.</div>
            </div>

            <div class="form-group">
                <label class="form-label" for="<%= txtSurname.ClientID %>">
                    Surname <span class="req" aria-hidden="true">*</span>
                </label>
                <asp:TextBox ID="txtSurname" runat="server" CssClass="form-control"
                             autocomplete="family-name" required="required" />
            </div>

            <div class="form-group">
                <label class="form-label" for="<%= txtFirstName.ClientID %>">
                    First name <span class="req" aria-hidden="true">*</span>
                </label>
                <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-control"
                             autocomplete="given-name" required="required" />
            </div>

            <div class="form-group">
                <label class="form-label" for="<%= txtOtherNames.ClientID %>">
                    Other names
                </label>
                <asp:TextBox ID="txtOtherNames" runat="server" CssClass="form-control"
                             autocomplete="additional-name" />
                <div class="form-hint">Optional â€” leave blank if not applicable.</div>
            </div>

            <hr style="border:none;border-top:1px solid var(--border);margin:28px 0;" />

            <h3 class="card-title" style="margin-bottom:4px;">Contact</h3>
            <p class="card-sub">We will use this email for all communication.</p>

            <div class="form-group">
                <label class="form-label" for="<%= txtEmail.ClientID %>">
                    Email address <span class="req" aria-hidden="true">*</span>
                </label>
                <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control"
                             TextMode="Email" autocomplete="email" required="required" />
            </div>

            <div class="form-group">
                <label class="form-label" for="<%= txtEmailConfirm.ClientID %>">
                    Confirm email address <span class="req" aria-hidden="true">*</span>
                </label>
                <asp:TextBox ID="txtEmailConfirm" runat="server" CssClass="form-control"
                             TextMode="Email" autocomplete="email" required="required" />
            </div>

            <hr style="border:none;border-top:1px solid var(--border);margin:28px 0;" />

            <h3 class="card-title" style="margin-bottom:4px;">Password</h3>
            <p class="card-sub">Minimum 8 characters. Use a mix of letters and numbers.</p>

            <div class="form-group">
                <label class="form-label" for="<%= txtPassword.ClientID %>">
                    Password <span class="req" aria-hidden="true">*</span>
                </label>
                <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control"
                             TextMode="Password" autocomplete="new-password" required="required" />
            </div>

            <div class="form-group">
                <label class="form-label" for="<%= txtPasswordConfirm.ClientID %>">
                    Confirm password <span class="req" aria-hidden="true">*</span>
                </label>
                <asp:TextBox ID="txtPasswordConfirm" runat="server" CssClass="form-control"
                             TextMode="Password" autocomplete="new-password" required="required" />
            </div>

            <hr style="border:none;border-top:1px solid var(--border);margin:28px 0;" />

            <h3 class="card-title" style="margin-bottom:4px;">Security check</h3>
            <p class="card-sub">Solve the simple sum below to confirm you are human.</p>

            <div class="form-group">
                <label class="form-label" for="<%= txtResult.ClientID %>">
                    <asp:Literal ID="litStamp1" runat="server" /> + <asp:Literal ID="litStamp2" runat="server" /> = <span class="req" aria-hidden="true">*</span>
                </label>
                <asp:TextBox ID="txtResult" runat="server" CssClass="form-control"
                             inputmode="numeric" MaxLength="3" required="required"
                             style="max-width:140px;" />
            </div>

            <asp:HiddenField ID="hfCsrf" runat="server" />
            <asp:HiddenField ID="hfFrom" runat="server" />

            <div style="display:flex;gap:12px;flex-wrap:wrap;margin-top:28px;">
                <asp:Button ID="btnSave" runat="server" Text="Create account"
                    CssClass="btn btn--primary btn--lg"
                    OnClick="btnSave_Click"
                    OnClientClick="return PSCIMS.btnLoading(this, 'Creating your accountâ€¦');" />
                <asp:HyperLink ID="lnkSignIn" runat="server"
                    CssClass="btn btn--outline btn--lg"
                    Text="Already have an account?">
                </asp:HyperLink>
            </div>

        </div>

        <p style="margin-top:24px;font-size:13px;color:var(--text-muted);text-align:center;">
            By creating an account you agree to the Commission's terms and confirm that
            all information you provide is accurate.
        </p>

    </div>

</asp:Content>*/



/*using System;
using System.Web.UI;

namespace PSCIMS.jobs
{
    public partial class RegisterProfile : System.Web.UI.Page
    {
        /// <summary>
        /// Where to send the user after successful registration.
        /// Default: the main jobs login. If ?from=puio, return to puio.
        /// </summary>
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
            if (!IsPostBack)
            {
                Random rnd = new Random();
                int s1 = rnd.Next(1, 10);
                int s2 = rnd.Next(1, 10);

                litStamp1.Text = s1.ToString();
                litStamp2.Text = s2.ToString();

                ViewState["ExpectedStampSum"] = (s1 + s2).ToString();

                // Capture the "from" hint from the query string
                string qFrom = (Request.QueryString["from"] ?? "").Trim().ToLowerInvariant();
                if (qFrom == "puio") hfFrom.Value = "puio";

                // Point the "sign in" link at the right section
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
            string idNum       = txtIdNum.Text.Trim();
            string surname     = txtSurname.Text.Trim();
            string firstName   = txtFirstName.Text.Trim();
            string otherNames  = txtOtherNames.Text.Trim();
            string email       = txtEmail.Text.Trim();
            string emailConf   = txtEmailConfirm.Text.Trim();
            string password    = txtPassword.Text;
            string passwordCf  = txtPasswordConfirm.Text;
            string stampAnswer = txtResult.Text.Trim();

            // ---------- Validation ----------
            if (string.IsNullOrEmpty(idNum) ||
                string.IsNullOrEmpty(surname) ||
                string.IsNullOrEmpty(firstName) ||
                string.IsNullOrEmpty(email) ||
                string.IsNullOrEmpty(emailConf) ||
                string.IsNullOrEmpty(password) ||
                string.IsNullOrEmpty(passwordCf) ||
                string.IsNullOrEmpty(stampAnswer))
            {
                Fail("Please complete all required fields.");
                return;
            }

            if (!string.Equals(email, emailConf, StringComparison.OrdinalIgnoreCase))
            {
                Fail("The email addresses do not match.");
                return;
            }

            if (password.Length < 8)
            {
                Fail("Password must be at least 8 characters long.");
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

            // ---------- TODO: wire to your existing DAL ----------
            // int newId = ApplicantDal.Register(idNum, surname, firstName, otherNames, email, password);
            // if (newId > 0) { phSuccess.Visible = true; }
            // else { Fail("An account with that ID or email already exists."); }

            // Success for demo:
            phError.Visible = false;
            phSuccess.Visible = true;

            txtIdNum.Text = "";
            txtSurname.Text = "";
            txtFirstName.Text = "";
            txtOtherNames.Text = "";
            txtEmail.Text = "";
            txtEmailConfirm.Text = "";
            txtPassword.Text = "";
            txtPasswordConfirm.Text = "";
            txtResult.Text = "";
        }

        private void Fail(string message)
        {
            phError.Visible = true;
            phSuccess.Visible = false;
            litError.Text = message;
        }
    }
}*/ 

