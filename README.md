# PSCIMS Demo

A working demonstration of the **Public Service Commission Information Management System (PSCIMS)**, built with ASP.NET Web Forms, C# and SQL Server.

**Live Demo:** [https://pscims-demo.runasp.net/](https://pscims-demo.runasp.net/)

---

## Table of Contents

- [Overview](#overview)
- [Project Objectives](#project-objectives)
- [Features](#features)
- [Technology Stack](#technology-stack)
- [Project Structure](#project-structure)
- [Database Schema](#database-schema)
- [Getting Started](#getting-started)
- [Environment Configuration](#environment-configuration)
- [Theming and Design System](#theming-and-design-system)
- [Security Architecture](#security-architecture)
- [HTTP Security Headers](#http-security-headers)
- [Email and OTP Delivery](#email-and-otp-delivery)
- [Accessibility](#accessibility)
- [Production Build and Deployment](#production-build-and-deployment)
- [Available Configuration Settings](#available-configuration-settings)
- [Known Limitations](#known-limitations)
- [Project Information](#project-information)
- [License](#license)

---

## Overview

The PSCIMS Demo is a working prototype of the Public Service Commission's recruitment and applicant management system.

The project was developed as part of an industrial attachment at the Public Service Commission Kenya, with an emphasis on demonstrating how a government recruitment portal can be built with layered security, a clear role model, and a consistent applicant and administrator experience.

The implementation combines an ASP.NET Web Forms frontend, a service layer that isolates security and data access concerns into dedicated classes, a SQL Server backend, and a token-based design system supporting both light and dark appearance preferences.

## Project Objectives

The project focuses on:

- Demonstrating an applicant registration and application workflow.
- Implementing authentication, OTP verification and role-based authorisation.
- Establishing a verifiable application security architecture.
- Maintaining an audit trail of security-relevant events.
- Supporting light and dark appearance preferences.
- Maintaining compatibility with older mobile browsers through an ES5 client-side strategy.
- Establishing a maintainable architecture for future development.

## Features

### Applicant Experience

- Registration using national ID or passport number.
- Email OTP verification with a 10-minute expiry.
- Secure authentication with password hashing.
- Browse advertised jobs, internships and university vacancies.
- Submit applications with database-level duplicate prevention.
- Track application status through a personalised dashboard.
- Password reset request workflow.
- Responsive interface across desktop, tablet and mobile devices.

### Administrator Experience

- Dedicated administrator dashboard.
- Manage advertised positions through CRUD operations.
- Review submitted applications.
- Manage users and roles.
- Manage public-facing website content through a CMS-style editor.

### Security

- PBKDF2 password hashing with 100,000 iterations.
- Cryptographically generated, single-use OTP tokens.
- Atomic SQL-based OTP verification.
- CSRF token validation on state-changing forms.
- Dual-key login throttling based on IP address and user identifier.
- Role-based access control.
- Parameterised SQL queries.
- Input validation and HTML output encoding.
- Audit logging to file and database.
- HTTP security headers configured and checked on the live deployment.

### Browser and Device Compatibility

- ES5-compatible client-side JavaScript.
- Server-rendered HTML for initial page accessibility.
- iOS safe-area inset support.
- Responsive sidebar with an off-canvas mobile drawer.
- Print stylesheet for applications and reports.

### Progressive Web Application

- Web app manifest.
- Theme-aware application icons.
- Service worker registration.
- Apple touch icons and supporting PWA assets.

### Search Engine Optimisation

- Page-level SEO metadata managed through the master page.
- Open Graph metadata for social sharing.
- Twitter Cards.
- JSON-LD structured data for `GovernmentOrganization`.
- Canonical URLs and language alternates.

---

## Technology Stack

| Category | Technology |
|---|---|
| Framework | ASP.NET Web Forms |
| Runtime | .NET Framework 4.8 |
| Programming Language | C# 7.3 |
| Database | Microsoft SQL Server |
| Frontend | HTML5, CSS3, ES5 JavaScript |
| Styling | CSS Custom Properties |
| Typography | Inter, Merriweather, Material Symbols |
| Email Delivery | SMTP |
| Authentication | Forms Authentication |
| Password Security | PBKDF2 |
| Deployment | MonsterASP Shared Hosting |

---

## Project Structure

```text
PSCIMS/
│
├── Admin/
│   ├── Default.aspx
│   ├── ManageAdverts.aspx
│   ├── ManageContent.aspx
│   ├── ManageUsers.aspx
│   └── ViewApplications.aspx
│
├── App_Data/
│   └── # Application data and runtime logs
│
├── bin/
│   └── PSCIMS.dll
│
├── content/
│   ├── css/
│   │   ├── psc-components.css
│   │   ├── psc-fonts.css
│   │   ├── psc-print.css
│   │   └── psc-theme.css
│   │
│   ├── fonts/
│   │   └── # Self-hosted WOFF2 fonts
│   │
│   ├── img/
│   │   └── # Logos, favicons and PWA icons
│   │
│   └── js/
│       ├── psc-main.js
│       └── psc-tables.js
│
├── jobs/
│   ├── Default.aspx
│   ├── loginPage.aspx
│   ├── RegisterProfile.aspx
│   ├── VerifyOtp.aspx
│   ├── ForgotPassword.aspx
│   ├── ActiveJobsAdverts.aspx
│   ├── ActiveAdvertsInterns.aspx
│   ├── Application.aspx
│   ├── ApplicantDashboard.aspx
│   ├── AdvertStatusGlobal.aspx
│   ├── ELPEXT.aspx
│   ├── logout.aspx
│   ├── Error.aspx
│   └── NotFound.aspx
│
├── JobsContent/
│   ├── ActiveJobsAdvertsContent.ascx
│   └── DefaultContent.ascx
│
├── puio/
│   ├── Default.aspx
│   └── web.config
│
├── Shared/
│   ├── AdvertDal.cs
│   ├── ApplicantDal.cs
│   ├── AuditLogger.cs
│   ├── Config.cs
│   ├── ContentProvider.cs
│   ├── CsrfProtector.cs
│   ├── EmailService.cs
│   ├── InputSanitizer.cs
│   ├── LoginThrottle.cs
│   ├── OtpService.cs
│   ├── PasswordHasher.cs
│   ├── RoleGuard.cs
│   └── SessionContext.cs
│
├── Site.master
├── Site.master.cs
└── web.config
```

### Key Directories

| Directory | Responsibility |
|---|---|
| `Admin/` | Administrative dashboard and management modules |
| `jobs/` | Applicant-facing pages and recruitment workflows |
| `JobsContent/` | Reusable public-facing content controls |
| `puio/` | Public Universities and Independent Offices section |
| `Shared/` | Security services, data access and application utilities |
| `content/css/` | Stylesheets and design system |
| `content/js/` | Client-side functionality |
| `App_Data/` | Application data and runtime storage |

---

## Database Schema

The application uses seven SQL Server tables to support recruitment, applicant management, authentication and auditing.

| Table | Purpose |
|---|---|
| `Adverts` | Job, internship and fellowship advertisements |
| `Applicants` | Registered applicants and authentication information |
| `Applications` | Submitted applications |
| `Roles` | User role definitions |
| `OtpTokens` | One-time passwords for verification |
| `LoginAttempts` | Login throttling history |
| `AuditLog` | Security and business event records |

### Database Design Considerations

- Timestamps use `datetime2(8)`.
- `AuditLog` and `LoginAttempts` use `bigint` identity columns to accommodate growing records.
- `Applications` has a unique constraint on `(ApplicantId, AdvertId)` to prevent duplicate applications.
- `OtpTokens` uses `ExpiresAt` and `ConsumedAt` to support expiry and single-use enforcement.
- The default applicant role is assigned through `RoleId`, following the least-privilege principle.

---

## Getting Started

### Prerequisites

Ensure the following tools are installed:

- Visual Studio 2019 or 2022 with the ASP.NET and web development workload.
- .NET Framework 4.8 targeting pack.
- Microsoft SQL Server or SQL Server LocalDB.
- SQL Server Management Studio (recommended).
- Git.

### 1. Clone the Repository

```bash
git clone <repository-url>
```

### 2. Navigate to the Project Directory

```bash
cd PSCIMS
```

### 3. Open the Solution

Open `PSCIMS.sln` in Visual Studio.

### 4. Create the Database

Run the database schema script from the `database/` directory, if available.

Alternatively, execute the SQL schema in SQL Server Management Studio.

Confirm that the seven required tables have been created.

### 5. Configure the Application

Open `web.config` and configure:

- `DatabaseTarget`
- The corresponding database connection string.
- SMTP credentials.

Refer to [Environment Configuration](#environment-configuration).

### 6. Run the Application

Press **F5** in Visual Studio.

The application should be accessible at:

```text
https://localhost:44300/jobs/Default.aspx
```

### 7. Create the First Administrator Account

The application does not automatically create an administrator account.

To promote an existing verified applicant:

1. Register through `/jobs/RegisterProfile.aspx`.
2. Complete email OTP verification.
3. Execute the following SQL statement in the database:

```sql
UPDATE dbo.Applicants
SET RoleId = 2
WHERE Email = 'you@example.com';
```

4. Sign in again to access the administrator section.

**Security note:** Only perform this operation through an authorised database administration account. Do not expose administrator role promotion through public registration.

---

## Environment Configuration

The application supports multiple database targets through `web.config`.

### Database Targets

| Configuration | Connection String |
|---|---|
| `MonsterASP` | `PSCIMS_MonsterASP` |
| `Azure` | `PSCIMS_Azure` |
| `LocalDB` | `PSCIMS_LocalDB` |

The active database target is selected using the `DatabaseTarget` application setting.

If the selected connection string is unavailable, the configuration resolver falls back to LocalDB and then to the legacy `PSCIMS` connection string, where configured.

### Example Configuration

```xml
<appSettings>
    <add key="DatabaseTarget" value="LocalDB" />

    <add key="PSCIMS_LocalDB"
         value="Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=PSCIMS;Integrated Security=True;MultipleActiveResultSets=True;Connection Timeout=5" />

    <add key="PSCIMS_MonsterASP"
         value="Server=...;Database=...;User Id=...;Password=...;Encrypt=False;MultipleActiveResultSets=True;" />

    <add key="SmtpHost" value="smtp.gmail.com" />
    <add key="SmtpPort" value="587" />
    <add key="SmtpUser" value="your@gmail.com" />
    <add key="SmtpPass" value="your-app-password" />
    <add key="SmtpFrom" value="your@gmail.com" />
</appSettings>
```

**Important security considerations:**

- Never commit real database connection strings or SMTP passwords to GitHub.
- Use Gmail App Passwords rather than regular account passwords.
- Where supported, store production credentials in the hosting provider's secure configuration settings.
- Do not expose database credentials or SMTP configuration to client-side JavaScript.

---

## Theming and Design System

The application uses CSS custom properties to maintain consistent styling across the master page and content pages.

The primary design tokens are defined in:

```text
content/css/psc-theme.css
```

### Light Theme

```css
:root {
    --accent: #c9a227;
    --accent-hover: #b08d1f;
    --bg: #f5f5f5;
    --surface: #ffffff;
    --text: #0f0f0f;
    --text-muted: #6b7280;
    --border: #e5e7eb;
}
```

### Dark Theme

```css
html[data-theme="dark"] {
    --accent: #e0b544;
    --bg: #0a0a0a;
    --surface: #141414;
    --text: #f5f5f5;
    --text-muted: #9ca3af;
    --border: #262626;
}
```

The complete token system includes additional surface, text, border, interaction and component-level variables.

To modify the primary accent colour, update:

- `--accent`
- `--accent-hover`

Components referencing these variables inherit the updated values automatically.

### Theme Initialisation

Theme initialisation takes place before the page paints.

An inline ES5 script in `Site.master`, inside the `<head>` element:

1. Reads the saved theme preference from `localStorage` using the `pscims-theme` key.
2. Falls back to the operating system preference through `matchMedia`.
3. Applies the appropriate `data-theme` attribute to the HTML element.

This minimises the flash of incorrectly themed content during page loading.

### Progressive Web App Manifest

The application uses a web app manifest located at:

```text
Content/img/site.webmanifest
```

Supporting assets include:

- Apple touch icons.
- Browser favicons.
- Progressive web application icons.

These assets maintain consistent PSC branding across browser tabs and supported installation environments.

---

## Security Architecture

Security controls are implemented through dedicated classes in the `Shared/` directory.

### Security Controls

| Security Control | Implementation |
|---|---|
| Password Hashing | `PasswordHasher.cs` |
| OTP Verification | `OtpService.cs` |
| CSRF Protection | `CsrfProtector.cs` |
| Login Throttling | `LoginThrottle.cs` |
| Audit Logging | `AuditLogger.cs` |
| Input Validation | `InputSanitizer.cs` |
| Role-Based Access | `RoleGuard.cs` |
| SQL Injection Prevention | Parameterised SQL queries |
| Session Context | `SessionContext.cs` |

### Implementation Details

**Password Security**

`PasswordHasher.cs` implements PBKDF2 password hashing with:

- 100,000 iterations.
- 16-byte salt.
- 32-byte derived hash.
- Constant-time hash comparison.

**One-Time Passwords**

`OtpService.cs` implements:

- Six-digit OTP generation using a cryptographically secure random number generator.
- Ten-minute validity period.
- Single-use verification.
- Atomic SQL-based token consumption.

**CSRF Protection**

`CsrfProtector.cs` implements:

- Cryptographically generated 32-byte tokens.
- HttpOnly, Secure and SameSite cookie configuration.
- Constant-time token comparison.
- Validation on state-changing requests.

**Login Throttling**

`LoginThrottle.cs` implements dual-key throttling based on:

- Client IP address.
- User identifier.

The configured threshold is five failed attempts within fifteen minutes.

**Audit Logging**

`AuditLogger.cs` supports logging to both a file and SQL Server database.

The audit trail records security-related and business-related events.

**Input Validation**

`InputSanitizer.cs` implements regular-expression allow-lists for relevant fields, together with HTML encoding to reduce injection and cross-site scripting risks.

**Role-Based Access Control**

`RoleGuard.cs` protects administrator functionality by checking authentication and assigned roles.

Unauthenticated users are redirected, while authenticated users without the required permissions receive an access-denied response.

**SQL Injection Prevention**

Database operations use parameterised SQL queries rather than dynamically concatenated user input.

**Session Context**

`SessionContext.cs` sets SQL session context using `sp_set_session_context`, providing a foundation for future row-level security implementation.

### Security Design Considerations

The implementation uses several security-oriented design decisions:

- PBKDF2 provides password hashing through the .NET cryptographic libraries.
- Constant-time comparison reduces timing differences during hash and token validation.
- Atomic SQL operations prevent successful reuse of consumed OTP tokens.
- Dual-key throttling addresses repeated attempts against individual accounts and from individual IP addresses.
- SQL session context provides a foundation for future database-level access restrictions.

---

## HTTP Security Headers

HTTP security headers are configured in `web.config` and have been checked against the live deployment.

| Header | Configuration |
|---|---|
| `Content-Security-Policy` | Restricts resource loading, framing and form submission |
| `Strict-Transport-Security` | `max-age=63072000; includeSubDomains; preload` |
| `X-Frame-Options` | `DENY` |
| `X-Content-Type-Options` | `nosniff` |
| `Referrer-Policy` | `strict-origin-when-cross-origin` |
| `Permissions-Policy` | Disables unused browser APIs |
| `Cross-Origin-Opener-Policy` | `same-origin` |
| `Cross-Origin-Resource-Policy` | `same-origin` |

### Additional Security Configuration

The application also includes:

- Removal of `X-AspNet-Version` and `X-Powered-By` response headers.
- Configuration to suppress server-identifying response headers.
- HttpOnly authentication cookies.
- SameSite cookie restrictions.
- Secure authentication cookies in production.
- ViewState MAC validation.
- ViewState encryption.

These controls complement the application's authentication, authorisation and request validation mechanisms.

---

## Email and OTP Delivery

Email delivery is handled through `EmailService.cs` using SMTP.

The service supports email verification and password-reset communication.

### Default SMTP Configuration

| Setting | Value |
|---|---|
| SMTP Host | `smtp.gmail.com` |
| SMTP Port | `587` |
| Encryption | STARTTLS |
| Timeout | 10 seconds |
| Authentication | SMTP credentials |

### Configuration Keys

The following settings are configured in `web.config`:

```text
SmtpHost
SmtpPort
SmtpUser
SmtpPass
SmtpFrom
```

### Connectivity Probe

`Config.HasInternet()` performs a TCP connectivity probe against configured DNS servers before attempting email delivery.

The probe uses:

- `208.67.222.222`
- `8.8.8.8`

It checks connectivity on port 53 and caches the result for 30 seconds within the request context.

This is a demonstration-level connectivity check intended to provide clearer error reporting when SMTP delivery fails.

### OTP Lifecycle

The OTP workflow follows these steps:

1. `OtpService.Create` generates a six-digit code using `RNGCryptoServiceProvider`.
2. Previously unconsumed tokens for the same applicant and purpose are invalidated.
3. A new token is inserted into the database with a ten-minute expiry.
4. Expiry is calculated using SQL Server's `SYSUTCDATETIME()`.
5. `OtpService.Verify` executes an atomic `UPDATE ... OUTPUT` statement.
6. A successfully consumed token cannot be reused.

This approach ensures that OTP expiry and consumption are enforced at the database level rather than relying exclusively on application-side validation.

---

## Accessibility

Accessibility is considered throughout the application's interface and interaction design.

Implemented measures include:

- Skip-to-content navigation.
- Visible keyboard focus indicators using `:focus-visible`.
- ARIA attributes on toggles, drawers, alerts and notifications.
- Accessible navigation drawer with focus management.
- `aria-expanded` state for interactive navigation.
- Reduced-motion support through `prefers-reduced-motion`.
- Colour contrast considerations across light and dark themes.
- Responsive layouts for different screen sizes.
- Dedicated print stylesheet for applications and reports.

The project targets **WCAG 2.1 Level AA** accessibility requirements.

---

## Production Build and Deployment

### MonsterASP Deployment

The application is deployed using [MonsterASP](https://www.monsterasp.net/) shared hosting.

**Live deployment:**

[https://pscims-demo.runasp.net/](https://pscims-demo.runasp.net/)

### Deployment Steps

1. Push the project to a GitHub repository or prepare a local Release build.
2. Open the project in Visual Studio.
3. Right-click the project and select **Publish**.
4. Select the Web Deploy or FTP publishing method configured by the hosting provider.
5. Publish the application in Release configuration.
6. Configure `DatabaseTarget` to use `MonsterASP`.
7. Provide the production SQL Server connection string.
8. Configure SMTP credentials.
9. Confirm that the hosting application pool supports .NET Framework 4.8.
10. Test the deployed application and its authentication workflows.

### Build Configuration

| Setting | Value |
|---|---|
| Configuration | Release |
| Target Framework | .NET Framework 4.8 |
| Publish Method | Web Deploy or FTP |
| Application Pool | .NET CLR Version 4.0 |
| Pipeline Mode | Integrated |
| Database | Hosted SQL Server |

### Post-Deployment Checklist

- [ ] `jobs/Default.aspx` loads successfully.
- [ ] Login page accepts input and rejects invalid credentials.
- [ ] Registration creates an applicant record.
- [ ] OTP verification sends and validates email codes.
- [ ] Successful OTP verification redirects appropriately.
- [ ] `AuditLog` records authentication and registration events.
- [ ] Administrator accounts can access management pages.
- [ ] Non-administrator accounts cannot access protected administrator pages.
- [ ] HTTP security headers appear in browser developer tools.
- [ ] `Config.WhichDatabase()` returns the expected database target.
- [ ] SMTP delivery works with the configured credentials.

### Deployment to Other Hosting Providers

The application can also be deployed to Windows hosting providers supporting:

- ASP.NET Web Forms.
- .NET Framework 4.8.
- SQL Server connectivity.
- SMTP email delivery.

The deployment process remains similar, with database and SMTP settings adjusted to the hosting provider.

---

## Available Configuration Settings

Application settings are maintained in `web.config` under the `<appSettings>` section.

| Configuration Key | Purpose |
|---|---|
| `DatabaseTarget` | Selects the active database environment |
| `PSCIMS_LocalDB` | LocalDB connection string |
| `PSCIMS_MonsterASP` | MonsterASP connection string |
| `PSCIMS_Azure` | Azure SQL connection string |
| `SmtpHost` | SMTP server hostname |
| `SmtpPort` | SMTP server port |
| `SmtpUser` | SMTP authentication username |
| `SmtpPass` | SMTP authentication password |
| `SmtpFrom` | Outgoing email sender address |
| `FallbackContactEmail` | Contact email displayed on error pages |

---

## Known Limitations

The current implementation has several identified areas for improvement.

These limitations are documented to provide transparency about the application's current development state.

| Area | Current Status |
|---|---|
| Audit Logger File Path | `AuditLogger` writes to `C:\temp1\pscims_audit.log`. This path works locally but is restricted on shared hosting. Database audit logging remains operational. |
| Connectivity Probe | `Config.HasInternet()` uses TCP connectivity checks against DNS servers. A more robust production health-check mechanism would be preferable. |
| Database Schema | `Admin/ManageAdverts.aspx` references an `IsActive` column that is absent from the current database schema. |
| Error Handling | `Application.aspx` currently exposes `ex.Message` in certain error responses. This should be replaced with generic user-facing messages and server-side logging. |
| Automated Testing | An automated test suite has not yet been implemented. Testing currently relies on documented manual test scenarios. |

### Planned Improvements

Future development should prioritise:

- Moving file-based audit logging to an application-relative directory such as `App_Data/logs`.
- Replacing the connectivity probe with a more reliable health-check mechanism.
- Reconciling database schema differences.
- Standardising exception handling across application workflows.
- Introducing automated unit and integration tests.
- Expanding deployment and security testing.

---

## Project Information

**Project:** PSCIMS Demo

**Institution:** Public Service Commission Kenya

**Developer:** Hillary Oduor

**Development Context:** Industrial Attachment

**Technology:** ASP.NET Web Forms, C#, SQL Server

**Deployment:** [MonsterASP Live Demo](https://pscims-demo.runasp.net/)

### References

- [Public Service Commission Kenya](https://www.publicservice.go.ke/)
- [PSCIMS Official Recruitment Portal](https://pscims.publicservice.go.ke/)
- [PSCIMS Demonstration Environment](https://pscims-demo.runasp.net/)

---

## License

This project is an internal demonstration and educational project developed during an industrial attachment at the Public Service Commission Kenya.

It is not licensed for redistribution or commercial reuse without the appropriate authorisation.

---

*Developed by Hillary Oduor | Public Service Commission Kenya*