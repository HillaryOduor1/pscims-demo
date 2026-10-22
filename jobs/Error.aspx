<%@ Page Title="Error" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" %>
<%@ Import Namespace="PSCIMS.Shared" %>

<asp:Content ID="PageTitleC" ContentPlaceHolderID="PageTitle" runat="server">Error</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
    <div class="state">
        <p class="state__mark state__mark--info" aria-hidden="true">&mdash;</p>
        <h1 class="state__title"><%= ContentProvider.Get("Empty_Title", "No applications yet") %></h1>
        <p class="state__desc">
            <%= ContentProvider.Get("Empty_Desc", "You have not submitted any applications. Browse the current adverts to get started.") %>
        </p>
        <p class="state__desc">
            <asp:Literal ID="litMessage" runat="server" />
        </p>
        <div class="state__actions">
            <a href="ActiveJobsAdverts.aspx" class="btn btn--primary">Browse advertised jobs</a>
        </div>
    </div>
</asp:Content>