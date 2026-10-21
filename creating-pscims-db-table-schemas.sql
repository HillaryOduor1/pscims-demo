USE [pscims-db];
GO

CREATE TABLE dbo.Applicants (
    Id              INT IDENTITY(1,1) PRIMARY KEY,
    IdNumber        NVARCHAR(20)  NOT NULL UNIQUE,
    Surname         NVARCHAR(100) NOT NULL,
    FirstName       NVARCHAR(100) NOT NULL,
    OtherNames      NVARCHAR(150) NULL,
    Email           NVARCHAR(200) NOT NULL UNIQUE,
    PasswordHash    NVARCHAR(255) NOT NULL,
    PasswordSalt    NVARCHAR(64)  NOT NULL,
    CreatedAt       DATETIME2     NOT NULL DEFAULT SYSUTCDATETIME(),
    LastLoginAt     DATETIME2     NULL,
    IsActive        BIT           NOT NULL DEFAULT 1
);
CREATE INDEX IX_Applicants_Email ON dbo.Applicants(Email);
GO

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
GO

CREATE TABLE dbo.Applications (
    Id              INT IDENTITY(1,1) PRIMARY KEY,
    ApplicantId     INT           NOT NULL,
    AdvertId        INT           NOT NULL,
    Status          NVARCHAR(30)  NOT NULL DEFAULT 'submitted',
    SubmittedAt     DATETIME2     NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAt       DATETIME2     NULL,
    Notes           NVARCHAR(MAX) NULL,
    CONSTRAINT FK_Applications_Applicant
        FOREIGN KEY (ApplicantId) REFERENCES dbo.Applicants(Id),
    CONSTRAINT FK_Applications_Advert
        FOREIGN KEY (AdvertId)    REFERENCES dbo.Adverts(Id),
    CONSTRAINT UQ_App_Applicant_Advert
        UNIQUE (ApplicantId, AdvertId)
);
GO

INSERT INTO dbo.Adverts (Reference, Title, Ministry, Location, Kind, OpensOn, ClosesOn) VALUES
('PSC/HRM/01/2026', 'Senior Human Resource Management Officer', 'Public Service Commission', 'Nairobi', 'job',        '2026-09-01', '2026-10-12'),
('MOH/CS/02/2026',  'Chief Pharmacist',                        'Ministry of Health',        'Nairobi', 'job',        '2026-09-01', '2026-10-05'),
('MOE/TCH/03/2026', 'Senior Teacher — Mathematics',            'Ministry of Education',     'Kisumu',  'job',        '2026-09-10', '2026-10-19'),
('TNT/ACC/04/2026', 'Principal Accountant',                    'The National Treasury',     'Nairobi', 'job',        '2026-09-15', '2026-10-01'),
('MOI/ICT/05/2026', 'Information Communication Technology Officer', 'Ministry of Interior', 'Mombasa', 'job',        '2026-09-12', '2026-10-08'),
('PSC/ICT/06/2026', 'ICT Officer I',                           'Public Service Commission', 'Nairobi', 'job',        '2026-08-01', '2026-09-26'),
('PSC/PSIP/01/2026','PSIP Internship — Human Resources',       'Public Service Commission', 'Nairobi', 'internship', '2026-09-01', '2026-10-08'),
('MOH/PSIP/02/2026','PSIP Internship — Public Health',         'Ministry of Health',        'Nairobi', 'internship', '2026-09-01', '2026-10-03'),
('MOE/PSIP/03/2026','PSIP Internship — Education Management',  'Ministry of Education',     'Nakuru',  'internship', '2026-09-05', '2026-10-16'),
('TNT/PSIP/04/2026','PSIP Internship — Public Finance',        'The National Treasury',     'Nairobi', 'internship', '2026-09-08', '2026-09-29');
GO

SELECT 'Schema created' AS Result;
SELECT COUNT(*) AS AdvertsCount FROM dbo.Adverts;
GO