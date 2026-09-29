using System;

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
            if (!IsPostBack)
            {
                // Make the "create an account" link section-aware
                string regUrl = Section == "puio"
                    ? ResolveUrl("~/jobs/RegisterProfile.aspx?from=puio")
                    : ResolveUrl("~/jobs/RegisterProfile.aspx");
                lnkRegister.NavigateUrl = regUrl;
                lnkRegister.Attributes["data-nav"] = "data-nav";

                lnkForgot.NavigateUrl = ResolveUrl("~/jobs/ForgotPassword.aspx");
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string id = txtId.Text.Trim();
            string pwd = txtPassword.Text;

            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(pwd))
            {
                phError.Visible = true;
                litError.Text = "Please enter both your ID / Passport Number and password.";
                return;
            }

            // ---------- TODO: wire to existing DAL ----------
            // if (ApplicantAuth.Validate(id, pwd))
            // {
            //     FormsAuthentication.SetAuthCookie(id, false);
            //     string target = Section == "puio"
            //         ? "~/puio/Default.aspx"
            //         : "~/jobs/ApplicantDashboard.aspx";
            //     Response.Redirect(target);
            // }

            phError.Visible = true;
            litError.Text = "Authentication is not yet wired to the backend.";
        }
    }
}

/*using System;

namespace PSCIMS.jobs
{
    public partial class loginPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e) { }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string id = txtId.Text.Trim();
            string pwd = txtPassword.Text;

            // TODO: keep your existing DAL / authentication call here.
            // e.g. if (ApplicantAuth.Validate(id, pwd)) {
            //          FormsAuthentication.SetAuthCookie(id, false);
            //          Response.Redirect("ApplicantDashboard.aspx");
            //      }

            // Placeholder for now so the page renders in IIS:
            phError.Visible = true;
            litError.Text = "Authentication logic to be wired to existing backend.";
        }
    }
}*/