using System;

namespace PSCIMS.jobs
{
    public partial class DefaultPartial : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Prevent the master from being involved — this page has no master.
        }
    }
}