<%@ Page Title="Register" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="RegisterProfile.aspx.cs" Inherits="PSCIMS.jobs.RegisterProfile" %>

<asp:Content ID="PageTitleC" ContentPlaceHolderID="PageTitle" runat="server">Register</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">

    <div style="max-width:720px;margin:0 auto;">

        <div class="mb-6">
            <h2 class="page-title">Create your applicant profile</h2>
            <p class="page-sub">
                Register once with your ID or Passport Number. You can then apply to any
                advertised position — public service, public universities, or independent offices.
            </p>
        </div>

        <asp:PlaceHolder ID="phError" runat="server" Visible="false">
            <div class="alert alert--danger" role="alert">
                <div class="alert__title">Could not complete registration</div>
                <asp:Literal ID="litError" runat="server" />
            </div>
        </asp:PlaceHolder>

        <asp:PlaceHolder ID="phSuccess" runat="server" Visible="false">
            <div class="alert alert--success" role="alert">
                <div class="alert__title">Registration successful</div>
                Your applicant profile has been created. You can now
                <asp:HyperLink ID="lnkSuccessSignIn" runat="server" Text="sign in" />
                to apply for advertised positions.
            </div>
        </asp:PlaceHolder>

        <div class="card" style="padding:28px;">

            <h3 class="card-title" style="margin-bottom:4px;">Personal information</h3>
            <p class="card-sub">As they appear on your national ID or passport.</p>

            <div class="form-group">
                <label class="form-label" for="<%= txtIdNum.ClientID %>">
                    ID / Passport Number <span class="req" aria-hidden="true">*</span>
                </label>
                <asp:TextBox ID="txtIdNum" runat="server" CssClass="form-control"
                             MaxLength="20" autocomplete="off" inputmode="numeric" required="required" />
                <div class="form-hint">Enter digits only, no spaces or slashes.</div>
            </div>

            <div class="form-group">
                <label class="form-label" for="<%= txtSurname.ClientID %>">
                    Surname <span class="req" aria-hidden="true">*</span>
                </label>
                <asp:TextBox ID="txtSurname" runat="server" CssClass="form-control"
                             autocomplete="family-name" required="required" />
            </div>

            <div class="form-group">
                <label class="form-label" for="<%= txtFirstName.ClientID %>">
                    First name <span class="req" aria-hidden="true">*</span>
                </label>
                <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-control"
                             autocomplete="given-name" required="required" />
            </div>

            <div class="form-group">
                <label class="form-label" for="<%= txtOtherNames.ClientID %>">
                    Other names
                </label>
                <asp:TextBox ID="txtOtherNames" runat="server" CssClass="form-control"
                             autocomplete="additional-name" />
                <div class="form-hint">Optional — leave blank if not applicable.</div>
            </div>

            <hr style="border:none;border-top:1px solid var(--border);margin:28px 0;" />

            <h3 class="card-title" style="margin-bottom:4px;">Contact</h3>
            <p class="card-sub">We will use this email for all communication.</p>

            <div class="form-group">
                <label class="form-label" for="<%= txtEmail.ClientID %>">
                    Email address <span class="req" aria-hidden="true">*</span>
                </label>
                <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control"
                             TextMode="Email" autocomplete="email" required="required" />
            </div>

            <div class="form-group">
                <label class="form-label" for="<%= txtEmailConfirm.ClientID %>">
                    Confirm email address <span class="req" aria-hidden="true">*</span>
                </label>
                <asp:TextBox ID="txtEmailConfirm" runat="server" CssClass="form-control"
                             TextMode="Email" autocomplete="email" required="required" />
            </div>

            <hr style="border:none;border-top:1px solid var(--border);margin:28px 0;" />

            <h3 class="card-title" style="margin-bottom:4px;">Password</h3>
            <p class="card-sub">Minimum 8 characters. Use a mix of letters and numbers.</p>

            <div class="form-group">
                <label class="form-label" for="<%= txtPassword.ClientID %>">
                    Password <span class="req" aria-hidden="true">*</span>
                </label>
                <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control"
                             TextMode="Password" autocomplete="new-password" required="required" />
            </div>

            <div class="form-group">
                <label class="form-label" for="<%= txtPasswordConfirm.ClientID %>">
                    Confirm password <span class="req" aria-hidden="true">*</span>
                </label>
                <asp:TextBox ID="txtPasswordConfirm" runat="server" CssClass="form-control"
                             TextMode="Password" autocomplete="new-password" required="required" />
            </div>

            <hr style="border:none;border-top:1px solid var(--border);margin:28px 0;" />

            <h3 class="card-title" style="margin-bottom:4px;">Security check</h3>
            <p class="card-sub">Solve the simple sum below to confirm you are human.</p>

            <div class="form-group">
                <label class="form-label" for="<%= txtResult.ClientID %>">
                    <asp:Literal ID="litStamp1" runat="server" /> + <asp:Literal ID="litStamp2" runat="server" /> = <span class="req" aria-hidden="true">*</span>
                </label>
                <asp:TextBox ID="txtResult" runat="server" CssClass="form-control"
                             inputmode="numeric" MaxLength="3" required="required"
                             style="max-width:140px;" />
            </div>

            <asp:HiddenField ID="hfFrom" runat="server" />

            <div style="display:flex;gap:12px;flex-wrap:wrap;margin-top:28px;">
                <asp:Button ID="btnSave" runat="server" Text="Create account"
                    CssClass="btn btn--primary btn--lg"
                    OnClick="btnSave_Click" />
                <asp:HyperLink ID="lnkSignIn" runat="server"
                    CssClass="btn btn--outline btn--lg"
                    Text="Already have an account?">
                </asp:HyperLink>
            </div>

        </div>

        <p style="margin-top:24px;font-size:13px;color:var(--text-muted);text-align:center;">
            By creating an account you agree to the Commission's terms and confirm that
            all information you provide is accurate.
        </p>

    </div>

</asp:Content>