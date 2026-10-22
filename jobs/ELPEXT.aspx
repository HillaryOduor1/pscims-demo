<%@ Page Title="Emerging Leaders Fellowship" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ELPEXT.aspx.cs" Inherits="PSCIMS.jobs.ELPEXT" %>
<%@ Import Namespace="PSCIMS.Shared" %>

<asp:Content ID="PageTitleC" ContentPlaceHolderID="PageTitle" runat="server">Emerging Leaders Fellowship</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="MainContent" runat="server">

    <div class="flex-between mb-6">
        <div>
            <h2 class="page-title"><%= ContentProvider.Get("ELP_Title", "Public Service Emerging Leaders Fellowship") %></h2>
            <p class="page-sub"><%= ContentProvider.Get("ELP_Sub", "A flagship programme developing the next generation of public service leaders in Kenya.") %></p>
        </div>
        <a href="RegisterProfile.aspx" class="btn btn--primary" data-nav>
            Apply for the fellowship
        </a>
    </div>

    <div class="card mb-6">
        <h3 class="card-title"><%= ContentProvider.Get("ELP_About_Title", "About the programme") %></h3>
        <p style="line-height:1.75;color:var(--text);margin:0 0 12px;">
            <%= ContentProvider.Get("ELP_About_Text1", "The Public Service Emerging Leaders Fellowship (ELP) is a competitive programme that identifies, develops, and mentors outstanding young professionals committed to excellence in the Kenyan public service. Fellows are placed in strategic positions across ministries, state departments, and agencies for the duration of the programme.") %>
        </p>
        <p style="line-height:1.75;color:var(--text);margin:0;">
            <%= ContentProvider.Get("ELP_About_Text2", "Fellows receive structured training in public sector leadership, ethics, policy formulation, and service delivery, alongside a mentorship attachment with a senior public officer.") %>
        </p>
    </div>

    <div class="stat-grid">
        <div class="stat">
            <div class="stat__value"><%= ContentProvider.Get("ELP_Stat_1", "12") %></div>
            <div class="stat__label"><%= ContentProvider.Get("ELP_Stat_1_Label", "Months Duration") %></div>
        </div>
        <div class="stat">
            <div class="stat__value"><%= ContentProvider.Get("ELP_Stat_2", "50") %></div>
            <div class="stat__label"><%= ContentProvider.Get("ELP_Stat_2_Label", "Fellows per Cohort") %></div>
        </div>
        <div class="stat">
            <div class="stat__value"><%= ContentProvider.Get("ELP_Stat_3", "3") %></div>
            <div class="stat__label"><%= ContentProvider.Get("ELP_Stat_3_Label", "Cohorts to Date") %></div>
        </div>
        <div class="stat">
            <div class="stat__value">&mdash;</div>
            <div class="stat__label">Next Application Window</div>
        </div>
    </div>

    <div class="card mb-6">
        <h3 class="card-title">Programme components</h3>
        <ul class="prose-list">
            <li><strong>Placement:</strong> Structured attachment to a ministry, state department, or agency.</li>
            <li><strong>Mentorship:</strong> One-on-one pairing with a senior public officer.</li>
            <li><strong>Training:</strong> Leadership, ethics, and public policy curriculum.</li>
            <li><strong>Rotation:</strong> Cross-functional exposure to service delivery and policy work.</li>
            <li><strong>Evaluation:</strong> Quarterly performance reviews and a capstone project.</li>
        </ul>
    </div>

    <div class="card mb-6">
        <h3 class="card-title">Eligibility criteria</h3>
        <ul class="prose-list">
            <li>Kenyan citizen with a valid national ID or passport.</li>
            <li>Holder of a Master's degree from a recognised university, or a Bachelor's degree with at least five (5) years of relevant professional experience.</li>
            <li>Aged 40 years or below at the time of application.</li>
            <li>Demonstrated leadership potential and commitment to public service.</li>
            <li>Clean record of professional and personal conduct.</li>
        </ul>
    </div>

    <div class="card mb-6">
        <h3 class="card-title">Application process</h3>
        <ol class="prose-list">
            <li>Register or sign in to PSCIMS using your ID / Passport Number.</li>
            <li>Complete the online fellowship application form when the window is open.</li>
            <li>Upload certified copies of academic certificates and transcripts.</li>
            <li>Provide three referees with valid contact details.</li>
            <li>Submit before the advert closure date.</li>
            <li>Shortlisted candidates will be invited for a competitive assessment centre.</li>
        </ol>
    </div>

    <div class="alert alert--warning">
        <div class="alert__title">Note</div>
        The next application window has not yet opened. Register now to receive notification when the ELP advert is published.
    </div>

    <div class="state">
        <h3 class="state__title">Applications open soon</h3>
        <p class="state__desc">
            The next ELP cohort application window has not yet been announced.
            Register your profile now and we will notify you when applications open.
        </p>
        <div class="state__actions">
            <a href="RegisterProfile.aspx" class="btn btn--primary" data-nav>Register for updates</a>
            <a href="Default.aspx" class="btn btn--outline" data-nav>Return to dashboard</a>
        </div>
    </div>

</asp:Content>