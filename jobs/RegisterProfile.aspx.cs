using System;
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
}