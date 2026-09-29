<%@ Control Language="C#" AutoEventWireup="true" %>

<div class="flex-between mb-6">
    <div>
        <h2 class="page-title">Public Universities &amp; Independent Offices</h2>
        <p class="page-sub">
            The Public Service Commission coordinates recruitment for public universities
            and independent offices. Register once and apply to any advertised vacancy.
        </p>
    </div>
    <a href="<%= ResolveUrl("~/jobs/RegisterProfile.aspx") %>" class="btn btn--primary btn--lg" data-nav>
        Register as a new applicant
    </a>
</div>

<!--<div class="alert alert-info" style="margin-bottom:24px;">
    <div class="alert__title">Shared applicant profile</div>
    Applicants to public universities and independent offices use the same applicant
    profile as those applying for mainstream public service positions. If you already
    have a PSCIMS account,
    <a href="<%= ResolveUrl("~/jobs/loginPage.aspx") %>" data-nav>sign in</a>
    to continue.
</div>-->

<div class="stat-grid">
    <div class="stat">
        <div class="stat__value">7</div>
        <div class="stat__label">Public Universities Hiring</div>
    </div>
    <div class="stat">
        <div class="stat__value">12</div>
        <div class="stat__label">Independent Offices</div>
    </div>
    <div class="stat">
        <div class="stat__value">26</div>
        <div class="stat__label">Open Adverts</div>
    </div>
    <div class="stat">
        <div class="stat__value">&mdash;</div>
        <div class="stat__label">Your Applications</div>
    </div>
</div>

<h3 class="card-title" style="font-size:17px;margin-bottom:12px;">Quick Actions</h3>

<div class="action-grid mb-6">

    <a href="<%= ResolveUrl("~/puio/ActiveJobsAdverts.aspx") %>" class="action-card" data-nav>
        <p class="action-card__title">Advertised Jobs</p>
        <p class="action-card__desc">Browse current vacancies across public universities and independent offices.</p>
    </a>

    <a href="<%= ResolveUrl("~/jobs/RegisterProfile.aspx") %>" class="action-card" data-nav>
        <p class="action-card__title">Register / Sign Up</p>
        <p class="action-card__desc">New user? Create an applicant profile using your ID or Passport number.</p>
    </a>

    <a href="<%= ResolveUrl("~/jobs/loginPage.aspx") %>" class="action-card" data-nav>
        <p class="action-card__title">Login</p>
        <p class="action-card__desc">Sign in to manage your applications and update your details.</p>
    </a>

    <a href="<%= ResolveUrl("~/jobs/loginPage.aspx") %>" class="action-card" data-nav>
        <p class="action-card__title">Application Status</p>
        <p class="action-card__desc">Check the progress of applications you have submitted.</p>
    </a>

</div>

<div class="card mb-6">
    <h3 class="card-title">Quick Guiding Steps</h3>
    <p class="card-sub">Read before you begin your application.</p>
    <ol style="line-height:1.85;padding-left:20px;margin:0;color:var(--text);">
        <li>All first-time users are required to <strong>register</strong> by providing ID / Passport Number, Surname, current Email address and a password.</li>
        <li>To apply for any advertised opportunity, <strong>log in</strong> using the ID / Passport Number and the password created in step 1.</li>
        <li>Applicants <strong>MUST</strong> provide complete information — personal details, professional and academic qualifications, experience, membership to professional bodies, referees. <strong>Incomplete applications will not be considered.</strong></li>
        <li>Print and keep a copy of your <strong>Feedback Report</strong> (application summary) by clicking on the Report tab on the Application Menu.</li>
        <li>You may amend or revisit your application at any time <strong>before the advert closure date</strong>.</li>
    </ol>
</div>

<div class="alert alert--warning">
    <div class="alert__title">Disclaimer</div>
    Section 100(4) of the Public Service Commission Act 2017 provides that a person who
    gives false or misleading information to the Commission is, on conviction, liable to
    a fine not exceeding Kshs. 200,000 or to imprisonment for a term not exceeding two
    years, or both.
</div>

<div class="card mb-6">
    <h3 class="card-title">Inquiries</h3>
    <p class="card-sub">For assistance with your application.</p>
    <p style="margin:0 0 8px;color:var(--text);">
        <strong>Email:</strong>
        <a href="mailto:pscict@publicservice.go.ke">pscict@publicservice.go.ke</a>
        — please include your ID / Passport Number and full name.
    </p>
    <p style="margin:0;color:var(--text);">
        <strong>Landline:</strong> +254 (020) 2223901 &nbsp;·&nbsp; 254 20 2227471<br />
        <strong>Call Centre:</strong> 020 4865000
    </p>
</div>

<div class="card">
    <h3 class="card-title">Resources</h3>
    <p class="card-sub">Download the user manual and guide.</p>
    <div style="display:flex;gap:20px;flex-wrap:wrap;">
        <a href="<%= ResolveUrl("~/images/GPCIS_USER_MANUAL.pdf") %>"
           target="_blank" rel="noopener"
           class="btn btn--outline">
            Download User Manual
        </a>
        <a href="<%= ResolveUrl("~/images/USER_GUIDE_GPCIS.pdf") %>"
           target="_blank" rel="noopener"
           class="btn btn--outline">
            Download User Guide
        </a>
    </div>
</div>