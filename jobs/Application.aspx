<%@ Page Title="Submit Application" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Application.aspx.cs" Inherits="PSCIMS.jobs.Application" %>
<%@ Import Namespace="PSCIMS.Shared" %>

<asp:Content ID="PageTitleC" ContentPlaceHolderID="PageTitle" runat="server">Submit Application</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
    <div class="form-shell">

        <div class="mb-6">
            <h2 class="page-title">Submit Application</h2>
            <p class="page-sub">You are about to apply for the following position.</p>
        </div>

        <asp:PlaceHolder ID="phError" runat="server" Visible="false">
            <div class="alert alert--danger" role="alert">
                <div class="alert__title">Cannot process application</div>
                <asp:Literal ID="litError" runat="server" />
            </div>
        </asp:PlaceHolder>

        <asp:PlaceHolder ID="phSuccess" runat="server" Visible="false">
            <div class="alert alert--success" role="alert">
                <div class="alert__title">Application Submitted!</div>
                Your application has been successfully submitted. You can view the status in your dashboard.
            </div>
            <div style="margin-top:24px;">
                <a href="ApplicantDashboard.aspx" class="btn btn--primary">Go to My Dashboard</a>
            </div>
        </asp:PlaceHolder>

        <asp:PlaceHolder ID="phForm" runat="server">
            <div class="card mb-6">
                <div class="flex-between">
                    <div>
                        <h3 class="card-title"><asp:Literal ID="litJobTitle" runat="server" /></h3>
                        <p class="card-sub" style="margin-bottom:0;"><asp:Literal ID="litJobMinistry" runat="server" /></p>
                    </div>
                    <span class="badge badge--open">Open</span>
                </div>
            </div>

            <div class="card" style="padding:28px;">
                <h3 class="card-title">Confirm your details</h3>
                <p class="card-sub">By clicking submit, you confirm that the details in your profile are correct and up-to-date.</p>

                <div class="form-actions form-actions--start">
                    <asp:Button ID="btnSubmit" runat="server" Text="Submit Application"
                        CssClass="btn btn--primary btn--lg" OnClick="btnSubmit_Click"
                        OnClientClick="return PSCIMS.btnLoading(this, 'Submitting…');" />
                    <a href="ApplicantDashboard.aspx" class="btn btn--outline btn--lg">Cancel</a>
                </div>
            </div>
        </asp:PlaceHolder>
    </div>
</asp:Content>