using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using PSCIMS.Shared;

namespace PSCIMS.Admin
{
    public partial class ManageAdverts : System.Web.UI.Page
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
            }
        }

        private void BindGrid()
        {
            using (var conn = new SqlConnection(Config.ConnectionString))
            using (var cmd = new SqlCommand("SELECT Id, Title, ClosesOn, IsActive FROM dbo.Adverts ORDER BY Id DESC", conn))
            {
                using (var da = new SqlDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    gvAdverts.DataSource = dt;
                    gvAdverts.DataBind();
                }
            }
        }

        protected void btnAddNew_Click(object sender, EventArgs e)
        {
            hfAdvertId.Value = "";
            txtTitle.Text = "";
            txtDesc.Text = "";
            txtClosingDate.Text = "";
            phForm.Visible = true;
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            phForm.Visible = false;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            using (var conn = new SqlConnection(Config.ConnectionString))
            using (var cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                
                if (string.IsNullOrEmpty(hfAdvertId.Value))
                {
                    cmd.CommandText = "INSERT INTO dbo.Adverts (Reference, Title, Ministry, Location, Description, OpensOn, ClosesOn, IsActive) " +
                                      "VALUES (@Ref, @T, 'Admin', 'Nairobi', @D, SYSUTCDATETIME(), @C, 1)";
                    cmd.Parameters.AddWithValue("@Ref", "ADM/" + DateTime.Now.Ticks.ToString().Substring(10));
                }
                else
                {
                    cmd.CommandText = "UPDATE dbo.Adverts SET Title = @T, Description = @D, ClosesOn = @C WHERE Id = @Id";
                    cmd.Parameters.AddWithValue("@Id", int.Parse(hfAdvertId.Value));
                }

                cmd.Parameters.AddWithValue("@T", txtTitle.Text.Trim());
                cmd.Parameters.AddWithValue("@D", txtDesc.Text.Trim());
                cmd.Parameters.AddWithValue("@C", DateTime.Parse(txtClosingDate.Text));

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            phForm.Visible = false;
            BindGrid();
            phMsg.Visible = true;
            litMsg.Text = "Advert saved successfully.";
        }

        protected void gvAdverts_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditAdvert")
            {
                int id = Convert.ToInt32(e.CommandArgument);
                LoadAdvert(id);
            }
            else if (e.CommandName == "ToggleStatus")
            {
                int id = Convert.ToInt32(e.CommandArgument);
                using (var conn = new SqlConnection(Config.ConnectionString))
                using (var cmd = new SqlCommand("UPDATE dbo.Adverts SET IsActive = CASE WHEN IsActive = 1 THEN 0 ELSE 1 END WHERE Id = @Id", conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                BindGrid();
            }
            else if (e.CommandName == "DeleteAdvert")
            {
                int id = Convert.ToInt32(e.CommandArgument);
                using (var conn = new SqlConnection(Config.ConnectionString))
                using (var cmd = new SqlCommand("DELETE FROM dbo.Adverts WHERE Id = @Id", conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                BindGrid();
            }
        }

        private void LoadAdvert(int id)
        {
            using (var conn = new SqlConnection(Config.ConnectionString))
            using (var cmd = new SqlCommand("SELECT Title, Description, ClosesOn FROM dbo.Adverts WHERE Id = @Id", conn))
            {
                cmd.Parameters.AddWithValue("@Id", id);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        hfAdvertId.Value = id.ToString();
                        txtTitle.Text = reader.IsDBNull(0) ? "" : reader.GetString(0);
                        txtDesc.Text = reader.IsDBNull(1) ? "" : reader.GetString(1);
                        txtClosingDate.Text = reader.IsDBNull(2) ? "" : reader.GetDateTime(2).ToString("yyyy-MM-dd");
                        phForm.Visible = true;
                    }
                }
            }
        }
    }
}