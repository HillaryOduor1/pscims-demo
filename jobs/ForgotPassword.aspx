<%@ Page Title="Forgot Password" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ForgotPassword.aspx.cs" Inherits="PSCIMS.jobs.ForgotPassword" %>

<asp:Content ID="PageTitleC" ContentPlaceHolderID="PageTitle" runat="server">Forgot Password</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">

    <div class="auth-wrap">
        <div class="auth-card">
            <div class="auth-card__head">
                <img src="<%= ResolveUrl("~/Content/img/psc-logo.png") %>" alt="Public Service Commission" onerror="this.style.display='none'" />
                <h1>Reset your password</h1>
                <p>Enter your ID or email and we'll send you a reset code.</p>
            </div>

            <asp:PlaceHolder ID="phError" runat="server" Visible="false">
                <div class="alert alert--danger" role="alert">
                    <asp:Literal ID="litError" runat="server" />
                </div>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="phRequest" runat="server">
                <div class="form-group">
                    <label class="form-label" for="<%= txtIdentifier.ClientID %>">
                        ID / Passport Number or Email <span class="req" aria-hidden="true">*</span>
                    </label>
                    <asp:TextBox ID="txtIdentifier" runat="server" CssClass="form-control" 
                                 placeholder="Enter your ID or email" required="required" />
                </div>

                <asp:HiddenField ID="hfCsrf" runat="server" />

                <asp:Button ID="btnSendCode" runat="server" Text="Send Reset Code"
                    CssClass="btn btn--primary btn--lg btn--block"
                    OnClick="btnSendCode_Click"
                    OnClientClick="return PSCIMS.btnLoading(this, 'Sending code…');" />
                    
                <div class="auth-card__footer">
                    Remembered it? <a href="<%= ResolveUrl("~/jobs/loginPage.aspx") %>">Sign in</a>
                </div>
            </asp:PlaceHolder>
            
        </div>
    </div>

    <%-- Reset Password Modal --%>
    <asp:PlaceHolder ID="phModal" runat="server" Visible="false">
        <div class="modal-overlay">
            <div class="modal" role="dialog" aria-modal="true" aria-labelledby="resetModalHeading">
                <h3 class="card-title" id="resetModalHeading">Enter reset code</h3>
                <p class="card-sub">
                    We sent a 6-digit code to your registered email. Please enter it below and choose a new password.
                </p>

                <asp:HiddenField ID="hfResetApplicantId" runat="server" />

                <div class="form-group">
                    <label class="form-label" for="<%= txtOtp.ClientID %>">
                        Verification Code <span class="req" aria-hidden="true">*</span>
                    </label>
                    <asp:TextBox ID="txtOtp" runat="server" CssClass="form-control" 
                                 MaxLength="6" inputmode="numeric" autocomplete="one-time-code"
                                 style="font-size:24px;letter-spacing:8px;text-align:center;" required="required" />
                </div>

                <div class="form-group">
                    <label class="form-label" for="<%= txtNewPassword.ClientID %>">
                        New Password <span class="req" aria-hidden="true">*</span>
                    </label>
                    <div class="password-wrapper">
                        <asp:TextBox ID="txtNewPassword" runat="server" CssClass="form-control"
                                     TextMode="Password" autocomplete="new-password" required="required" />
                        <button type="button" class="password-toggle" onclick="togglePasswordVisibility('<%= txtNewPassword.ClientID %>', this)" aria-label="Toggle password visibility">
                            <span class="material-symbols-outlined" aria-hidden="true">visibility</span>
                        </button>
                    </div>
                </div>

                <div class="form-group">
                    <label class="form-label" for="<%= txtConfirmPassword.ClientID %>">
                        Confirm New Password <span class="req" aria-hidden="true">*</span>
                    </label>
                    <div class="password-wrapper">
                        <asp:TextBox ID="txtConfirmPassword" runat="server" CssClass="form-control"
                                     TextMode="Password" autocomplete="new-password" required="required" />
                        <button type="button" class="password-toggle" onclick="togglePasswordVisibility('<%= txtConfirmPassword.ClientID %>', this)" aria-label="Toggle password visibility">
                            <span class="material-symbols-outlined" aria-hidden="true">visibility</span>
                        </button>
                    </div>
                </div>

                <div class="form-actions form-actions--start">
                    <asp:Button ID="btnResetPassword" runat="server" Text="Reset Password"
                        CssClass="btn btn--primary btn--lg"
                        OnClick="btnResetPassword_Click"
                        OnClientClick="return PSCIMS.btnLoading(this, 'Resetting…');" />
                    
                    <asp:Button ID="btnCancelModal" runat="server" Text="Cancel"
                        CssClass="btn btn--outline btn--lg"
                        OnClick="btnCancelModal_Click" CausesValidation="false" />
                </div>
            </div>
        </div>
    </asp:PlaceHolder>

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