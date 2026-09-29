<%@ Page Title="No results" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" %>

<asp:Content ID="PageTitleC" ContentPlaceHolderID="PageTitle" runat="server">No results</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">

    <div class="state">
        <p class="state__mark state__mark--info" aria-hidden="true">&mdash;</p>
        <h1 class="state__title">No applications yet</h1>
        <p class="state__desc">
            You have not submitted any applications. Browse the current adverts to get started.
        </p>
        <div class="state__actions">
            <a href="ActiveJobsAdverts.aspx" class="btn btn--primary">Browse advertised jobs</a>
        </div>
    </div>

</asp:Content>