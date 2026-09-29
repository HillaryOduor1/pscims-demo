<%@ Page Title="Page not found" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" %>

<asp:Content ID="PageTitleC" ContentPlaceHolderID="PageTitle" runat="server">Page not found</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">

    <div class="state">
        <p class="state__mark" aria-hidden="true">404</p>
        <h1 class="state__title">Page not found</h1>
        <p class="state__desc">
            The page you are looking for does not exist or may have been moved.
            Check the address, or use the links below to continue.
        </p>
        <div class="state__actions">
            <a href="Default.aspx" class="btn btn--primary">Return to dashboard</a>
            <a href="ActiveJobsAdverts.aspx" class="btn btn--outline">Browse advertised jobs</a>
        </div>
    </div>

</asp:Content>