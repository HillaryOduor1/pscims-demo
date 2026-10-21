<%@ Page Title="Sign in" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="loginPage.aspx.cs" Inherits="PSCIMS.jobs.loginPage" %>

<asp:Content ID="PageTitleC" ContentPlaceHolderID="PageTitle" runat="server">Sign in</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">

    <div class="auth-wrap">
        <div class="auth-card">
            <div class="auth-card__head">
                <img src="<%= ResolveUrl("~/Content/img/psc-logo.webp") %>" alt="Public Service Commission" onerror="this.style.display='none'" />
                <h1>Sign in to PSCIMS</h1>
                <p>Public Service Commission Information Management System</p>
            </div>

            <asp:PlaceHolder ID="phError" runat="server" Visible="false">
                <div class="alert alert--danger" role="alert">
                    <asp:Literal ID="litError" runat="server" />
                </div>
            </asp:PlaceHolder>

            <div class="form-group">
                <label class="form-label" for="<%= txtId.ClientID %>">
                    ID / Passport Number <span class="req" aria-hidden="true">*</span>
                </label>
                <asp:TextBox ID="txtId" runat="server" CssClass="form-control" MaxLength="20"
                             autocomplete="username" inputmode="numeric" required="required" />
            </div>

            <div class="form-group">
                <label class="form-label" for="<%= txtPassword.ClientID %>">
                    Password <span class="req" aria-hidden="true">*</span>
                </label>
                <div class="password-wrapper">
                    <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="form-control"
                                 autocomplete="current-password" required="required" />
                    <button type="button" class="password-toggle" onclick="togglePasswordVisibility('<%= txtPassword.ClientID %>', this)" aria-label="Toggle password visibility">
                        <span class="material-symbols-outlined">visibility</span>
                    </button>
                </div>
                <div class="form-hint">
                    <asp:HyperLink ID="lnkForgot" runat="server" Text="Forgot your password?" />
                </div>
            </div>

            <asp:HiddenField ID="hfCsrf" runat="server" />

            <%-- Changed to LinkButton so it renders an <a> tag --%>
            <asp:LinkButton ID="btnLogin" runat="server"
                CssClass="btn btn--primary btn--lg btn--block"
                OnClick="btnLogin_Click"
                OnClientClick="return PSCIMS.btnLoading(this, 'Signing in…');">
                Sign in
            </asp:LinkButton>

            <div class="auth-card__footer">
                New applicant?
                <asp:HyperLink ID="lnkRegister" runat="server" Text="Create an account" />
            </div>
        </div>
    </div>

    <script>
        function togglePasswordVisibility(inputId, btn) {
            var input = document.getElementById(inputId);
            var icon = btn.querySelector('.material-symbols-outlined');
            if (input.type === 'password') {
                input.type = 'text';
                icon.textContent = 'visibility_off';
            } else {
                input.type = 'password';
                icon.textContent = 'visibility';
            }
        }
    </script>

</asp:Content>