namespace PSCIMS.jobs
{
    public partial class RegisterProfile
    {
        protected global::System.Web.UI.WebControls.PlaceHolder phError;
        protected global::System.Web.UI.WebControls.Literal litError;

        protected global::System.Web.UI.WebControls.PlaceHolder phSuccess;
        protected global::System.Web.UI.WebControls.HyperLink lnkSuccessSignIn;

        protected global::System.Web.UI.WebControls.TextBox txtIdNum;
        protected global::System.Web.UI.WebControls.TextBox txtSurname;
        protected global::System.Web.UI.WebControls.TextBox txtFirstName;
        protected global::System.Web.UI.WebControls.TextBox txtOtherNames;

        protected global::System.Web.UI.WebControls.TextBox txtEmail;
        protected global::System.Web.UI.WebControls.TextBox txtEmailConfirm;

        protected global::System.Web.UI.WebControls.TextBox txtPassword;
        protected global::System.Web.UI.WebControls.TextBox txtPasswordConfirm;

        protected global::System.Web.UI.WebControls.Literal litStamp1;
        protected global::System.Web.UI.WebControls.Literal litStamp2;
        protected global::System.Web.UI.WebControls.TextBox txtResult;

        protected global::System.Web.UI.WebControls.HiddenField hfCsrf;
        protected global::System.Web.UI.WebControls.HiddenField hfFrom;

        // Changed to LinkButton
        protected global::System.Web.UI.WebControls.LinkButton btnSave;
        
        protected global::System.Web.UI.WebControls.HyperLink lnkSignIn;
    }
}