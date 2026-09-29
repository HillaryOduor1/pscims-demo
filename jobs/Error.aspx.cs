using System;
using System.Web;

namespace PSCIMS.jobs
{
    public partial class Error : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError();
            if (ex != null)
            {
                litMessage.Text = HttpUtility.HtmlEncode(ex.GetBaseException().Message);
            }
            else
            {
                litMessage.Text = "No error details available.";
            }
        }
    }
}