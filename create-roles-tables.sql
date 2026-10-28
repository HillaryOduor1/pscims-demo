-- 1. Create the Roles table
CREATE TABLE dbo.Roles (
    RoleId INT IDENTITY(1,1) PRIMARY KEY,
    RoleName NVARCHAR(50) NOT NULL UNIQUE,
    Description NVARCHAR(255)
);

-- Insert default roles
INSERT INTO dbo.Roles (RoleName, Description) VALUES 
('Applicant', 'Standard user who can apply for jobs'),
('Admin', 'Full system access, can manage users and content'),
('HR', 'Can view applications and manage adverts');

-- 2. Add RoleId and Contact fields to Applicants
ALTER TABLE dbo.Applicants 
ADD RoleId INT NULL,
    PhoneNumber NVARCHAR(20) NULL;

-- Set all existing users to Applicant (RoleId = 1)
UPDATE dbo.Applicants SET RoleId = 1 WHERE RoleId IS NULL;

-- Add foreign key
ALTER TABLE dbo.Applicants
ADD CONSTRAINT FK_Applicants_Roles FOREIGN KEY (RoleId) REFERENCES dbo.Roles(RoleId);

-- 3. Create the SiteContent table for CMS (removes hardcoded text)
CREATE TABLE dbo.SiteContent (
    ContentKey NVARCHAR(100) PRIMARY KEY,
    ContentValue NVARCHAR(MAX) NOT NULL,
    LastUpdated DATETIME DEFAULT SYSUTCDATETIME(),
    UpdatedBy INT NULL
);

-- Example inserts for your pages
INSERT INTO dbo.SiteContent (ContentKey, ContentValue) VALUES
('Home_WelcomeTitle', 'Welcome to PSCIMS'),
('Home_WelcomeSub', 'Your gateway to public service recruitment in Kenya.'),
('Home_Disclaimer', 'Section 100(4) of the Public Service Commission Act 2017 provides...');