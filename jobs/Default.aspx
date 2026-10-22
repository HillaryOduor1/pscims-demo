<%@ Page Title="Dashboard" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="PSCIMS.jobs.Default" %>
<%@ Import Namespace="PSCIMS.Shared" %>

<asp:Content ID="PageTitleC" ContentPlaceHolderID="PageTitle" runat="server">Dashboard</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">

    <div class="flex-between mb-6">
        <div>
            <h2 class="page-title"><%= ContentProvider.Get("Home_Title", "") %></h2>
            <p class="page-sub"><%= ContentProvider.Get("Home_Sub", "") %></p>
        </div>
        <a href="RegisterProfile.aspx" class="btn btn--primary btn--lg">
            <%= ContentProvider.Get("Home_RegisterBtn", "Register as a new applicant") %>
        </a>
    </div>

    <div class="stat-grid">
        <div class="stat">
            <div class="stat__value"><%= ContentProvider.Get("Home_Stat_OpenAdverts", "42") %></div>
            <div class="stat__label">Open Adverts</div>
        </div>
        <div class="stat">
            <div class="stat__value"><%= ContentProvider.Get("Home_Stat_Internships", "18") %></div>
            <div class="stat__label">Internships</div>
        </div>
        <div class="stat">
            <div class="stat__value"><%= ContentProvider.Get("Home_Stat_Universities", "7") %></div>
            <div class="stat__label">Universities Hiring</div>
        </div>
        <div class="stat">
            <div class="stat__value">&mdash;</div>
            <div class="stat__label">Your Applications</div>
        </div>
    </div>

    <h3 class="card-title" style="font-size:17px;margin-bottom:12px;"><%= ContentProvider.Get("Home_QuickActions_Title", "Quick Actions") %></h3>

    <div class="action-grid mb-6">
        <a href="RegisterProfile.aspx" class="action-card">
            <p class="action-card__title"><%= ContentProvider.Get("Home_Action_Reg_Title", "Register / Sign up") %></p>
            <p class="action-card__desc"><%= ContentProvider.Get("Home_Action_Reg_Desc", "Create your applicant profile using your ID or Passport number.") %></p>
        </a>
        <a href="loginPage.aspx" class="action-card">
            <p class="action-card__title"><%= ContentProvider.Get("Home_Action_SignIn_Title", "Sign in") %></p>
            <p class="action-card__desc"><%= ContentProvider.Get("Home_Action_SignIn_Desc", "Access your account to apply and manage applications.") %></p>
        </a>
        <a href="ActiveJobsAdverts.aspx" class="action-card">
            <p class="action-card__title"><%= ContentProvider.Get("Home_Action_Jobs_Title", "Advertised Jobs") %></p>
            <p class="action-card__desc"><%= ContentProvider.Get("Home_Action_Jobs_Desc", "Browse all currently open positions in the public service.") %></p>
        </a>
        <a href="ActiveAdvertsInternsInternshipExt.aspx" class="action-card">
            <p class="action-card__title"><%= ContentProvider.Get("Home_Action_Int_Title", "Internships") %></p>
            <p class="action-card__desc"><%= ContentProvider.Get("Home_Action_Int_Desc", "Current internship opportunities under the PSIP programme.") %></p>
        </a>
        <a href="loginPage.aspx" class="action-card">
            <p class="action-card__title"><%= ContentProvider.Get("Home_Action_Apps_Title", "My Applications") %></p>
            <p class="action-card__desc"><%= ContentProvider.Get("Home_Action_Apps_Desc", "View the status of applications you have submitted.") %></p>
        </a>
        <a href="AdvertStatusGlobal.aspx" class="action-card">
            <p class="action-card__title"><%= ContentProvider.Get("Home_Action_Status_Title", "Status of Advertised Posts") %></p>
            <p class="action-card__desc"><%= ContentProvider.Get("Home_Action_Status_Desc", "Check whether an advertised post is still open or closed.") %></p>
        </a>
    </div>

    <div class="card mb-6">
        <h3 class="card-title"><%= ContentProvider.Get("Home_Steps_Title", "Quick Guiding Steps") %></h3>
        <p class="card-sub"><%= ContentProvider.Get("Home_Steps_Sub", "Read before you begin your application.") %></p>
        <ol class="prose-list">
            <li><%= ContentProvider.Get("Home_Step_1", "All first-time users must <strong>register</strong> with ID / Passport Number, Surname, Email and a password.") %></li>
            <li><%= ContentProvider.Get("Home_Step_2", "To apply for any advertised opportunity, <strong>log in</strong> with your ID and password.") %></li>
            <li><%= ContentProvider.Get("Home_Step_3", "Provide all required information — <strong>incomplete applications will not be considered</strong>.") %></li>
            <li><%= ContentProvider.Get("Home_Step_4", "Print and keep a copy of your Feedback Report before the closure date.") %></li>
            <li><%= ContentProvider.Get("Home_Step_5", "You may amend your application at any time <strong>before the advert closure date</strong>.") %></li>
        </ol>
    </div>

    <div class="alert alert--warning">
        <div class="alert__title"><%= ContentProvider.Get("Home_Disclaimer_Title", "Disclaimer") %></div>
        <%= ContentProvider.Get("Home_Disclaimer_Text", "Section 100(4) of the Public Service Commission Act 2017 provides that a person who gives false or misleading information to the Commission is, on conviction, liable to a fine not exceeding Kshs. 200,000 or to imprisonment for a term not exceeding two years, or both.") %>
    </div>

</asp:Content>