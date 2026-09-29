<%@ Page Title="Coming soon" Language="C#" AutoEventWireup="true" CodeBehind="ComingSoon.aspx.cs" Inherits="PSCIMS.jobs.ComingSoon" %>
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1, viewport-fit=cover" />
    <title>Coming soon — PSCIMS</title>
    <link rel="icon" type="image/x-icon" href="<%= ResolveUrl("~/favicon.ico") %>" />
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600;700;800&family=Merriweather:wght@400;700;900&display=swap" rel="stylesheet">
    <link href="https://fonts.googleapis.com/css2?family=Material+Symbols+Outlined:opsz,wght,FILL,GRAD@24,400,0,0&display=swap" rel="stylesheet" />
    <link href="<%= ResolveUrl("~/Content/css/psc-theme.css") %>" rel="stylesheet" />
    <script>
        (function () {
            try {
                var saved = localStorage.getItem('pscims-theme');
                var prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
                var theme = saved || (prefersDark ? 'dark' : 'light');
                document.documentElement.setAttribute('data-theme', theme);
            } catch (e) { }
        })();
    </script>
    <style>
        /* Minimal wrapper so this standalone page centers nicely */
        body.psc-standalone {
            min-height: 100vh;
            min-height: 100dvh;
            display: flex;
            align-items: center;
            justify-content: center;
            padding: 24px;
            background: var(--bg);
        }
        .psc-standalone-card {
            max-width: 560px;
            width: 100%;
            background: var(--surface);
            border: 1px solid var(--border);
            border-radius: var(--radius-lg);
            padding: 48px 40px;
            box-shadow: var(--shadow);
            text-align: center;
        }
        @media (max-width: 480px) {
            .psc-standalone-card { padding: 32px 24px; }
        }
        .psc-standalone-logo {
            width: 56px; height: 56px;
            margin: 0 auto 20px;
            border-radius: 12px;
            background: var(--accent-soft);
            padding: 8px;
            object-fit: contain;
        }
    </style>
</head>
<body class="psc-standalone">

    <div class="psc-standalone-card">

        <img class="psc-standalone-logo"
             src="<%= ResolveUrl("~/Content/img/psc-logo.png") %>"
             alt="Public Service Commission" />

        <p class="state__mark" aria-hidden="true">&#8230;</p>

        <h1 class="state__title"><asp:Literal ID="litTitle" runat="server" Text="This page is coming soon" /></h1>

        <p class="state__desc">
            <asp:Literal ID="litMessage" runat="server" />
        </p>

        <div class="state__actions">
            <a href="<%= ResolveUrl("~/jobs/Default.aspx") %>" class="btn btn--primary">
                Return to dashboard
            </a>
            <a href="<%= ResolveUrl("~/jobs/ActiveJobsAdverts.aspx") %>" class="btn btn--outline">
                Browse advertised jobs
            </a>
        </div>

        <p style="margin-top:32px;font-size:13px;color:var(--text-subtle);">
            Need this urgently? Contact
            <a href="mailto:contactcentre@publicservice.go.ke">contactcentre@publicservice.go.ke</a>
            or call <strong>020 4865000</strong>.
        </p>

    </div>

</body>
</html>