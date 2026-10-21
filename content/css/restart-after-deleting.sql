
GO

-- Clear all data, preserving table structure
DELETE FROM dbo.Applications;
DELETE FROM dbo.OtpTokens;
DELETE FROM dbo.AuditLog;
DELETE FROM dbo.LoginAttempts;
DELETE FROM dbo.Applicants;
GO

-- Reset identity columns so new users start from Id=1 again
DBCC CHECKIDENT ('dbo.Applicants',    RESEED, 0);
DBCC CHECKIDENT ('dbo.OtpTokens',     RESEED, 0);
DBCC CHECKIDENT ('dbo.Applications',  RESEED, 0);
DBCC CHECKIDENT ('dbo.AuditLog',      RESEED, 0);
DBCC CHECKIDENT ('dbo.LoginAttempts', RESEED, 0);
GO

-- Confirm
SELECT 'Applicants' AS T, COUNT(*) AS Rows FROM dbo.Applicants
UNION ALL SELECT 'OtpTokens', COUNT(*) FROM dbo.OtpTokens
UNION ALL SELECT 'Adverts',   COUNT(*) FROM dbo.Adverts;
GO