<%@ Page Title="Admin Dashboard" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="PSCIMS.Admin.Default" %>

<asp:Content ID="PageTitleC" ContentPlaceHolderID="PageTitle" runat="server">Admin Dashboard</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">
    <div class="mb-6">
        <h2 class="page-title">Administrator Dashboard</h2>
        <p class="page-sub">Welcome back. Manage users, content, and adverts from here.</p>
    </div>

    <div class="stat-grid">
        <a class="stat stat--link" href="ManageUsers.aspx">
            <div class="stat__value">Manage</div>
            <div class="stat__label">Users &amp; Roles</div>
        </a>
        <a class="stat stat--link" href="ManageAdverts.aspx">
            <div class="stat__value">Manage</div>
            <div class="stat__label">Job Adverts</div>
        </a>
        <a class="stat stat--link" href="ManageContent.aspx">
            <div class="stat__value">Manage</div>
            <div class="stat__label">Site Content</div>
        </a>
        <a class="stat stat--link" href="ViewApplications.aspx">
            <div class="stat__value">Review</div>
            <div class="stat__label">Applications</div>
        </a>
    </div>
</asp:Content>