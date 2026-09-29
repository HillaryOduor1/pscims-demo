<%@ Page Title="Something went wrong" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="Error.aspx.cs" Inherits="PSCIMS.jobs.Error" %>

<asp:Content ID="PageTitleC" ContentPlaceHolderID="PageTitle" runat="server">Error</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">

    <div class="state">
        <p class="state__mark state__mark--error" aria-hidden="true">500</p>
        <h1 class="state__title">Something went wrong</h1>
        <p class="state__desc">
            We could not complete your request. Please try again. If the problem persists, contact the
            PSC Call Centre on <strong>020 4865000</strong> or email
            <a href="mailto:contactcentre@publicservice.go.ke">contactcentre@publicservice.go.ke</a>.
        </p>
        <p class="state__desc" style="font-size:13px;color:var(--text-subtle);">
            Reference: <asp:Literal ID="litMessage" runat="server" />
        </p>
        <div class="state__actions">
            <a href="Default.aspx" class="btn btn--primary">Return to dashboard</a>
            <a href="https://www.publicservice.go.ke" class="btn btn--outline" target="_blank" rel="noopener">Go to PSC website</a>
        </div>
    </div>

</asp:Content>