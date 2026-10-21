using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using PSCIMS.Shared;

namespace PSCIMS.Admin
{
    public partial class ManageContent : System.Web.UI.Page
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
            string filterPrefix = ddlPages.SelectedValue; // e.g., "Home_", "Int_"

            using (var conn = new SqlConnection(Config.ConnectionString))
            using (var cmd = new SqlCommand("SELECT ContentKey, ContentValue, LastUpdated FROM dbo.SiteContent WHERE ContentKey LIKE @Prefix + '%' ORDER BY ContentKey", conn))
            {
                cmd.Parameters.AddWithValue("@Prefix", filterPrefix);
                
                using (var da = new SqlDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    gvContent.DataSource = dt;
                    gvContent.DataBind();
                }
            }
        }

        protected void ddlPages_SelectedIndexChanged(object sender, EventArgs e)
        {
            gvContent.EditIndex = -1;
            BindGrid();
        }

        protected void gvContent_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditContent")
            {
                string key = e.CommandArgument.ToString();
                LoadContent(key);
            }
        }

        private void LoadContent(string key)
        {
            using (var conn = new SqlConnection(Config.ConnectionString))
            using (var cmd = new SqlCommand("SELECT ContentKey, ContentValue FROM dbo.SiteContent WHERE ContentKey = @K", conn))
            {
                cmd.Parameters.AddWithValue("@K", key);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        hfContentKey.Value = reader.GetString(0);
                        txtKeyName.Text = reader.GetString(0);
                        txtContentValue.Text = reader.GetString(1);
                        phForm.Visible = true;
                    }
                }
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            phForm.Visible = false;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(hfContentKey.Value)) return;

            using (var conn = new SqlConnection(Config.ConnectionString))
            using (var cmd = new SqlCommand("UPDATE dbo.SiteContent SET ContentValue = @V, LastUpdated = SYSUTCDATETIME() WHERE ContentKey = @K", conn))
            {
                cmd.Parameters.AddWithValue("@V", txtContentValue.Text.Trim());
                cmd.Parameters.AddWithValue("@K", hfContentKey.Value);
                conn.Open();
                cmd.ExecuteNonQuery();
            }

            phForm.Visible = false;
            BindGrid();
            phMsg.Visible = true;
            litMsg.Text = "Content updated successfully.";
        }
    }
}