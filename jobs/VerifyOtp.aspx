<%@ Page Title="Verify your email" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="VerifyOtp.aspx.cs" Inherits="PSCIMS.jobs.VerifyOtp" %>

<asp:Content ID="PageTitleC" ContentPlaceHolderID="PageTitle" runat="server">Verify your email</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">

    <div class="auth-wrap">
        <div class="auth-card">
            <div class="auth-card__head">
                <img src="<%= ResolveUrl("~/Content/img/psc-logo.webp") %>"
                     alt="Public Service Commission"
                     onerror="this.style.display='none'" />
                <h1>Verify your email</h1>
                <p>We sent a 6-digit code to <strong><asp:Literal ID="litEmail" runat="server" /></strong></p>
            </div>

            <asp:PlaceHolder ID="phError" runat="server" Visible="false">
                <div id="alertBox" runat="server" class="alert alert--danger" role="alert">
                    <asp:Literal ID="litError" runat="server" />
                </div>
            </asp:PlaceHolder>

            <div class="form-group">
                <label class="form-label" for="<%= txtCode.ClientID %>">
                    Verification code <span class="req" aria-hidden="true">*</span>
                </label>
                <asp:TextBox ID="txtCode" runat="server" CssClass="form-control"
                             MaxLength="6" inputmode="numeric" autocomplete="one-time-code"
                             required="required"
                             style="font-size:24px;letter-spacing:8px;text-align:center;" />
                <div class="form-hint">Code expires in 10 minutes.</div>
            </div>

            <asp:HiddenField ID="hfCsrf" runat="server" />

            <asp:Button ID="btnVerify" runat="server" Text="Verify email"
                CssClass="btn btn--primary btn--lg btn--block"
                OnClick="btnVerify_Click"
                OnClientClick="return PSCIMS.btnLoading(this, 'Verifying…');" />

            <div class="auth-card__footer">
                Didn't receive it?
                <asp:LinkButton ID="lnkResend" runat="server" Text="Resend code"
                    OnClick="lnkResend_Click"
                    OnClientClick="return PSCIMS.btnLoading(this, 'Sending new code…');" />
            </div>
        </div>
    </div>

</asp:Content>