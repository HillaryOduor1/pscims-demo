<%@ Page Title="Admin Dashboard" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="PSCIMS.Admin.Default" %>

<asp:Content ID="PageTitleC" ContentPlaceHolderID="PageTitle" runat="server">Admin Dashboard</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
    <div class="mb-6">
        <h2 class="page-title">Administrator Dashboard</h2>
        <p class="page-sub">Welcome back. Manage users, content, and adverts from here.</p>
    </div>

    <div class="stat-grid">
        <div class="stat">
            <div class="stat__value">Manage</div>
            <div class="stat__label"><a href="ManageUsers.aspx">Users & Roles</a></div>
        </div>
        <div class="stat">
            <div class="stat__value">Manage</div>
            <div class="stat__label"><a href="ManageAdverts.aspx">Job Adverts</a></div>
        </div>
        <div class="stat">
            <div class="stat__value">Manage</div>
            <div class="stat__label"><a href="ManageContent.aspx">Site Content</a></div>
        </div>
        <div class="stat">
            <div class="stat__value">Review</div>
            <div class="stat__label"><a href="ViewApplications.aspx">Applications</a></div>
        </div>
    </div>
</asp:Content>