USE [pscims-db];
GO

-- ============================================================
-- Drop existing (in dependency order)
-- ============================================================
IF OBJECT_ID('dbo.Applications','U')  IS NOT NULL DROP TABLE dbo.Applications;
IF OBJECT_ID('dbo.Adverts','U')       IS NOT NULL DROP TABLE dbo.Adverts;
IF OBJECT_ID('dbo.Applicants','U')    IS NOT NULL DROP TABLE dbo.Applicants;
IF OBJECT_ID('dbo.Roles','U')         IS NOT NULL DROP TABLE dbo.Roles;
IF OBJECT_ID('dbo.OtpTokens','U')     IS NOT NULL DROP TABLE dbo.OtpTokens;
IF OBJECT_ID('dbo.AuditLog','U')      IS NOT NULL DROP TABLE dbo.AuditLog;
IF OBJECT_ID('dbo.LoginAttempts','U') IS NOT NULL DROP TABLE dbo.LoginAttempts;
GO

-- ============================================================
-- Roles
-- ============================================================
CREATE TABLE dbo.Roles (
    Id              INT IDENTITY(1,1) PRIMARY KEY,
    Name            NVARCHAR(50) NOT NULL UNIQUE,   -- 'applicant' | 'reviewer' | 'admin'
    Description     NVARCHAR(200) NULL
);
INSERT INTO dbo.Roles (Name, Description) VALUES
('applicant', 'Regular job applicant'),
('reviewer',  'Can view applications for review'),
('admin',     'Full administrative access');

-- ============================================================
-- Applicants
-- ============================================================
CREATE TABLE dbo.Applicants (
    Id              INT IDENTITY(1,1) PRIMARY KEY,
    IdNumber        NVARCHAR(20)  NOT NULL UNIQUE,
    Surname         NVARCHAR(100) NOT NULL,
    FirstName       NVARCHAR(100) NOT NULL,
    OtherNames      NVARCHAR(150) NULL,
    Email           NVARCHAR(200) NOT NULL UNIQUE,
    EmailVerified   BIT           NOT NULL DEFAULT 0,
    PasswordHash    NVARCHAR(255) NOT NULL,
    PasswordSalt    NVARCHAR(64)  NOT NULL,
    RoleId          INT           NOT NULL DEFAULT 1,
    CreatedAt       DATETIME2     NOT NULL DEFAULT SYSUTCDATETIME(),
    LastLoginAt     DATETIME2     NULL,
    IsActive        BIT           NOT NULL DEFAULT 1,
    CONSTRAINT FK_Applicants_Role FOREIGN KEY (RoleId) REFERENCES dbo.Roles(Id)
);
CREATE INDEX IX_Applicants_Email ON dbo.Applicants(Email);

-- ============================================================
-- OTP tokens (email verification, password reset)
-- ============================================================
CREATE TABLE dbo.OtpTokens (
    Id              INT IDENTITY(1,1) PRIMARY KEY,
    ApplicantId     INT           NOT NULL,
    Code            NVARCHAR(10)  NOT NULL,
    Purpose         NVARCHAR(30)  NOT NULL,      -- 'email_verify' | 'password_reset'
    ExpiresAt       DATETIME2     NOT NULL,
    ConsumedAt      DATETIME2     NULL,
    CreatedAt       DATETIME2     NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_Otp_Applicant FOREIGN KEY (ApplicantId) REFERENCES dbo.Applicants(Id)
);
CREATE INDEX IX_Otp_Applicant ON dbo.OtpTokens(ApplicantId, Purpose, ExpiresAt);

-- ============================================================
-- Adverts
-- ============================================================
CREATE TABLE dbo.Adverts (
    Id              INT IDENTITY(1,1) PRIMARY KEY,
    Reference       NVARCHAR(50)  NOT NULL UNIQUE,
    Title           NVARCHAR(200) NOT NULL,
    Ministry        NVARCHAR(150) NOT NULL,
    Location        NVARCHAR(100) NOT NULL,
    Description     NVARCHAR(MAX) NULL,
    Kind            NVARCHAR(20)  NOT NULL DEFAULT 'job',
    OpensOn         DATE          NOT NULL,
    ClosesOn        DATE          NOT NULL,
    CreatedAt       DATETIME2     NOT NULL DEFAULT SYSUTCDATETIME()
);
CREATE INDEX IX_Adverts_ClosesOn ON dbo.Adverts(ClosesOn);

-- ============================================================
-- Applications (with RLS)
-- ============================================================
CREATE TABLE dbo.Applications (
    Id              INT IDENTITY(1,1) PRIMARY KEY,
    ApplicantId     INT           NOT NULL,
    AdvertId        INT           NOT NULL,
    Status          NVARCHAR(30)  NOT NULL DEFAULT 'submitted',
    SubmittedAt     DATETIME2     NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAt       DATETIME2     NULL,
    Notes           NVARCHAR(MAX) NULL,
    CONSTRAINT FK_Applications_Applicant FOREIGN KEY (ApplicantId) REFERENCES dbo.Applicants(Id),
    CONSTRAINT FK_Applications_Advert    FOREIGN KEY (AdvertId)    REFERENCES dbo.Adverts(Id),
    CONSTRAINT UQ_App_Applicant_Advert   UNIQUE (ApplicantId, AdvertId)
);
CREATE INDEX IX_Applications_Applicant ON dbo.Applications(ApplicantId);

-- ============================================================
-- Audit log
-- ============================================================
CREATE TABLE dbo.AuditLog (
    Id              BIGINT IDENTITY(1,1) PRIMARY KEY,
    EventType       NVARCHAR(50)  NOT NULL,     -- 'login' | 'register' | 'otp_verify' | ...
    ApplicantId     INT           NULL,
    IpAddress       NVARCHAR(45)  NULL,
    UserAgent       NVARCHAR(500) NULL,
    Detail          NVARCHAR(500) NULL,
    Success         BIT           NOT NULL,
    CreatedAt       DATETIME2     NOT NULL DEFAULT SYSUTCDATETIME()
);
CREATE INDEX IX_Audit_Applicant ON dbo.AuditLog(ApplicantId, CreatedAt);

-- ============================================================
-- Rate limiting — login attempts
-- ============================================================
CREATE TABLE dbo.LoginAttempts (
    Id              BIGINT IDENTITY(1,1) PRIMARY KEY,
    Identifier      NVARCHAR(100) NOT NULL,     -- IdNumber OR IP, whichever we're limiting
    Kind            NVARCHAR(20)  NOT NULL,     -- 'id' | 'ip'
    Success         BIT           NOT NULL,
    AttemptedAt     DATETIME2     NOT NULL DEFAULT SYSUTCDATETIME()
);
CREATE INDEX IX_LoginAttempts ON dbo.LoginAttempts(Identifier, Kind, AttemptedAt);

-- ============================================================
-- Seed adverts
-- ============================================================
INSERT INTO dbo.Adverts (Reference, Title, Ministry, Location, Kind, OpensOn, ClosesOn) VALUES
('PSC/HRM/01/2026', 'Senior Human Resource Management Officer', 'Public Service Commission', 'Nairobi', 'job',        '2026-09-01', '2026-12-12'),
('MOH/CS/02/2026',  'Chief Pharmacist',                        'Ministry of Health',        'Nairobi', 'job',        '2026-09-01', '2026-12-05'),
('MOE/TCH/03/2026', 'Senior Teacher — Mathematics',            'Ministry of Education',     'Kisumu',  'job',        '2026-09-10', '2026-12-19'),
('TNT/ACC/04/2026', 'Principal Accountant',                    'The National Treasury',     'Nairobi', 'job',        '2026-09-15', '2026-12-01'),
('MOI/ICT/05/2026', 'Information Communication Technology Officer', 'Ministry of Interior', 'Mombasa', 'job',        '2026-09-12', '2026-12-08'),
('PSC/PSIP/01/2026','PSIP Internship — Human Resources',       'Public Service Commission', 'Nairobi', 'internship', '2026-09-01', '2026-12-08'),
('MOH/PSIP/02/2026','PSIP Internship — Public Health',         'Ministry of Health',        'Nairobi', 'internship', '2026-09-01', '2026-12-03'),
('MOE/PSIP/03/2026','PSIP Internship — Education Management',  'Ministry of Education',     'Nakuru',  'internship', '2026-09-05', '2026-12-16'),
('TNT/PSIP/04/2026','PSIP Internship — Public Finance',        'The National Treasury',     'Nairobi', 'internship', '2026-09-08', '2026-12-29');
GO