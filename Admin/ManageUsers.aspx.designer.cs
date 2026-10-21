namespace PSCIMS.Admin
{
    public partial class ManageUsers
    {
        protected global::System.Web.UI.WebControls.LinkButton btnAddNew;
        protected global::System.Web.UI.WebControls.PlaceHolder phMsg;
        protected global::System.Web.UI.WebControls.Literal litMsg;
        protected global::System.Web.UI.WebControls.PlaceHolder phError;
        protected global::System.Web.UI.WebControls.Literal litError;
        protected global::System.Web.UI.WebControls.GridView gvUsers;
        
        // Modal controls
        protected global::System.Web.UI.WebControls.PlaceHolder phForm;
        protected global::System.Web.UI.WebControls.TextBox txtNewIdNum;
        protected global::System.Web.UI.WebControls.TextBox txtNewFirstName;
        protected global::System.Web.UI.WebControls.TextBox txtNewSurname;
        protected global::System.Web.UI.WebControls.TextBox txtNewEmail;
        protected global::System.Web.UI.WebControls.TextBox txtNewPassword;
        protected global::System.Web.UI.WebControls.DropDownList ddlNewRole;
        protected global::System.Web.UI.WebControls.Button btnCancel;
        protected global::System.Web.UI.WebControls.Button btnSave;
    }
}