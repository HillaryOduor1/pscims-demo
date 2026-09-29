<%@ Page Title="Sign in" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="loginPage.aspx.cs" Inherits="PSCIMS.jobs.loginPage" %>

<asp:Content ID="PageTitleC" ContentPlaceHolderID="PageTitle" runat="server">Sign in</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">

    <div class="auth-wrap">
        <div class="auth-card">
            <div class="auth-card__head">
                <img src="<%= ResolveUrl("~/Content/img/psc-logo.png") %>" alt="Public Service Commission" />
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
                <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="form-control"
                             autocomplete="current-password" required="required" />
                <div class="form-hint">
                    <asp:HyperLink ID="lnkForgot" runat="server" Text="Forgot your password?" />
                </div>
            </div>

            <asp:Button ID="btnLogin" runat="server" Text="Sign in"
                CssClass="btn btn--primary btn--lg btn--block"
                OnClick="btnLogin_Click" />

            <div class="auth-card__footer">
                New applicant?
                <asp:HyperLink ID="lnkRegister" runat="server" Text="Create an account" />
            </div>
        </div>
    </div>

</asp:Content>