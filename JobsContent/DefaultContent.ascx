<%@ Control Language="C#" AutoEventWireup="true" %>

<div class="flex-between mb-6">
    <div>
        <h2 class="page-title">Welcome to PSCIMS</h2>
        <p class="page-sub">Your gateway to public service recruitment in Kenya.</p>
    </div>
    <a href="RegisterProfile.aspx" class="btn btn--primary btn--lg" data-nav>
        Register as a new applicant
    </a>
</div>

<div class="stat-grid">
    <div class="stat">
        <div class="stat__value">42</div>
        <div class="stat__label">Open Adverts</div>
    </div>
    <div class="stat">
        <div class="stat__value">18</div>
        <div class="stat__label">Internships</div>
    </div>
    <div class="stat">
        <div class="stat__value">7</div>
        <div class="stat__label">Universities Hiring</div>
    </div>
    <div class="stat">
        <div class="stat__value">&mdash;</div>
        <div class="stat__label">Your Applications</div>
    </div>
</div>

<h3 class="card-title" style="font-size:17px;margin-bottom:12px;">Quick Actions</h3>

<div class="action-grid mb-6">

    <a href="RegisterProfile.aspx" class="action-card" data-nav>
        <p class="action-card__title">Register / Sign up</p>
        <p class="action-card__desc">Create your applicant profile using your ID or Passport number.</p>
    </a>

    <a href="loginPage.aspx" class="action-card" data-nav>
        <p class="action-card__title">Sign in</p>
        <p class="action-card__desc">Access your account to apply and manage applications.</p>
    </a>

    <a href="ActiveJobsAdverts.aspx" class="action-card" data-nav>
        <p class="action-card__title">Advertised Jobs</p>
        <p class="action-card__desc">Browse all currently open positions in the public service.</p>
    </a>

    <a href="ActiveAdvertsInternsInternshipExt.aspx" class="action-card" data-nav>
        <p class="action-card__title">Internships</p>
        <p class="action-card__desc">Current internship opportunities under the PSIP programme.</p>
    </a>

    <a href="loginPage.aspx" class="action-card" data-nav>
        <p class="action-card__title">My Applications</p>
        <p class="action-card__desc">View the status of applications you have submitted.</p>
    </a>

    <a href="AdvertStatusGlobal.aspx" class="action-card" data-nav>
        <p class="action-card__title">Status of Advertised Posts</p>
        <p class="action-card__desc">Check whether an advertised post is still open or closed.</p>
    </a>

</div>

<div class="card mb-6">
    <h3 class="card-title">Quick Guiding Steps</h3>
    <p class="card-sub">Read before you begin your application.</p>
    <ol style="line-height:1.85;padding-left:20px;margin:0;color:var(--text);">
        <li>All first-time users must <strong>register</strong> with ID / Passport Number, Surname, Email and a password.</li>
        <li>To apply for any advertised opportunity, <strong>log in</strong> with your ID and password.</li>
        <li>Provide all required information — <strong>incomplete applications will not be considered</strong>.</li>
        <li>Print and keep a copy of your Feedback Report before the closure date.</li>
        <li>You may amend your application at any time <strong>before the advert closure date</strong>.</li>
    </ol>
</div>

<div class="alert alert--warning">
    <div class="alert__title">Disclaimer</div>
    Section 100(4) of the Public Service Commission Act 2017 provides that a person who gives false or misleading
    information to the Commission is, on conviction, liable to a fine not exceeding Kshs. 200,000 or to imprisonment
    for a term not exceeding two years, or both.
</div>