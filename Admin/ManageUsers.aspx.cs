using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using PSCIMS.Shared;

namespace PSCIMS.Admin
{
    public partial class ManageUsers : System.Web.UI.Page
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
                BindRoleDropDown(ddlNewRole);
            }
        }

        private void BindGrid()
        {
            using (var conn = new SqlConnection(Config.ConnectionString))
            using (var cmd = new SqlCommand(
                "SELECT a.Id, a.FirstName, a.Surname, a.IdNumber, a.Email, a.RoleId, r.Name AS RoleName " +
                "FROM dbo.Applicants a " +
                "LEFT JOIN dbo.Roles r ON a.RoleId = r.Id " +
                "ORDER BY a.Id DESC", conn))
            {
                using (var da = new SqlDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    gvUsers.DataSource = dt;
                    gvUsers.DataBind();
                }
            }
        }

        private void BindRoleDropDown(DropDownList ddl)
        {
            using (var conn = new SqlConnection(Config.ConnectionString))
            using (var cmd = new SqlCommand("SELECT Id, Name FROM dbo.Roles", conn))
            {
                using (var da = new SqlDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    ddl.DataSource = dt;
                    ddl.DataTextField = "Name";
                    ddl.DataValueField = "Id";
                    ddl.DataBind();
                }
            }
        }

        protected void btnAddNew_Click(object sender, EventArgs e)
        {
            txtNewIdNum.Text = "";
            txtNewFirstName.Text = "";
            txtNewSurname.Text = "";
            txtNewEmail.Text = "";
            txtNewPassword.Text = "";
            ddlNewRole.SelectedIndex = 0;
            phForm.Visible = true;
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            phForm.Visible = false;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            string idNum = InputSanitizer.Clean(txtNewIdNum.Text, 20);
            string firstName = InputSanitizer.Clean(txtNewFirstName.Text, 100);
            string surname = InputSanitizer.Clean(txtNewSurname.Text, 100);
            string email = InputSanitizer.Clean(txtNewEmail.Text, 200).ToLowerInvariant();
            string password = txtNewPassword.Text;
            int roleId = Convert.ToInt32(ddlNewRole.SelectedValue);

            // Validate using the exact same rules as registration
            if (!InputSanitizer.IsValidId(idNum))
            {
                ShowError("ID / Passport Number must be 7-10 alphanumeric characters.");
                return;
            }

            if (!InputSanitizer.IsValidName(surname) || !InputSanitizer.IsValidName(firstName))
            {
                ShowError("Names must contain only letters, spaces, hyphens, apostrophes, or periods.");
                return;
            }

            if (!InputSanitizer.IsValidEmail(email))
            {
                ShowError("Please enter a valid email address.");
                return;
            }

            if (password.Length < 8 || password.Length > 128)
            {
                ShowError("Password must be between 8 and 128 characters.");
                return;
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(password, @"[A-Z]") ||
                !System.Text.RegularExpressions.Regex.IsMatch(password, @"[a-z]") ||
                !System.Text.RegularExpressions.Regex.IsMatch(password, @"\d"))
            {
                ShowError("Password must contain an uppercase letter, a lowercase letter, and a digit.");
                return;
            }

            try
            {
                string hash, salt;
                PasswordHasher.CreateHash(password, out hash, out salt);

                using (var conn = new SqlConnection(Config.ConnectionString))
                using (var cmd = new SqlCommand(
                    "INSERT INTO dbo.Applicants (IdNumber, Surname, FirstName, Email, EmailVerified, PasswordHash, PasswordSalt, RoleId, IsActive) " +
                    "VALUES (@Id, @Sn, @Fn, @Em, 1, @Hash, @Salt, @Role, 1)", conn))
                {
                    cmd.Parameters.AddWithValue("@Id", idNum);
                    cmd.Parameters.AddWithValue("@Sn", surname);
                    cmd.Parameters.AddWithValue("@Fn", firstName);
                    cmd.Parameters.AddWithValue("@Em", email);
                    cmd.Parameters.AddWithValue("@Hash", hash);
                    cmd.Parameters.AddWithValue("@Salt", salt);
                    cmd.Parameters.AddWithValue("@Role", roleId);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                phForm.Visible = false;
                BindGrid();
                ShowMessage("User created successfully. They can now log in.");
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627 || ex.Number == 2601)
                {
                    ShowError("A user with that ID Number or Email already exists.");
                }
                else
                {
                    ShowError("Database error: " + ex.Message);
                }
            }
            catch (Exception ex)
            {
                ShowError("Error creating user: " + ex.Message);
            }
        }

        protected void gvUsers_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvUsers.EditIndex = e.NewEditIndex;
            BindGrid();
            
            var ddl = (DropDownList)gvUsers.Rows[e.NewEditIndex].FindControl("ddlRoles");
            if (ddl != null)
            {
                BindRoleDropDown(ddl);
                using (var conn = new SqlConnection(Config.ConnectionString))
                using (var cmd = new SqlCommand("SELECT RoleId FROM dbo.Applicants WHERE Id = @Id", conn))
                {
                    cmd.Parameters.AddWithValue("@Id", gvUsers.DataKeys[e.NewEditIndex].Value);
                    conn.Open();
                    var roleId = cmd.ExecuteScalar();
                    if (roleId != null && roleId != DBNull.Value)
                        ddl.SelectedValue = roleId.ToString();
                }
            }
        }

        protected void gvUsers_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvUsers.EditIndex = -1;
            BindGrid();
        }

        protected void gvUsers_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            var ddl = (DropDownList)gvUsers.Rows[e.RowIndex].FindControl("ddlRoles");
            if (ddl != null)
            {
                int userId = Convert.ToInt32(gvUsers.DataKeys[e.RowIndex].Value);
                using (var conn = new SqlConnection(Config.ConnectionString))
                using (var cmd = new SqlCommand("UPDATE dbo.Applicants SET RoleId = @R WHERE Id = @Id", conn))
                {
                    cmd.Parameters.AddWithValue("@R", ddl.SelectedValue);
                    cmd.Parameters.AddWithValue("@Id", userId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            gvUsers.EditIndex = -1;
            BindGrid();
            ShowMessage("User role updated successfully.");
        }

        protected void gvUsers_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "SendOtp")
            {
                int userId = Convert.ToInt32(e.CommandArgument);
                try
                {
                    string email = ApplicantDal.GetEmail(userId);
                    string firstName = ApplicantDal.GetFirstName(userId);
                    string code = OtpService.Create(userId, "admin_resend");
                    EmailService.SendOtp(email, firstName, code);
                    ShowMessage("OTP successfully sent to " + email);
                }
                catch (Exception ex)
                {
                    ShowError("Could not send OTP: " + ex.Message);
                }
            }
            else if (e.CommandName == "ResetPassword")
            {
                int userId = Convert.ToInt32(e.CommandArgument);
                try
                {
                    string newPass = "Psc" + Guid.NewGuid().ToString().Substring(0, 6) + "!1"; // Ensure it meets complexity
                    
                    string hash, salt;
                    PasswordHasher.CreateHash(newPass, out hash, out salt);

                    using (var conn = new SqlConnection(Config.ConnectionString))
                    using (var cmd = new SqlCommand("UPDATE dbo.Applicants SET PasswordHash = @H, PasswordSalt = @S WHERE Id = @Id", conn))
                    {
                        cmd.Parameters.AddWithValue("@H", hash);
                        cmd.Parameters.AddWithValue("@S", salt);
                        cmd.Parameters.AddWithValue("@Id", userId);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }

                    ShowMessage("Password reset. Temporary password is: " + newPass + " (Please note this down!)");
                }
                catch (Exception ex)
                {
                    ShowError("Error resetting password: " + ex.Message);
                }
            }
        }

        private void ShowMessage(string msg)
        {
            phMsg.Visible = true;
            litMsg.Text = msg;
            phError.Visible = false;
        }

        private void ShowError(string msg)
        {
            phError.Visible = true;
            litError.Text = msg;
            phMsg.Visible = false;
        }
    }
}