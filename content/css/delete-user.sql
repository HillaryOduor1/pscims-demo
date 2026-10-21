
GO

-- Show what's there before we delete
SELECT 'Applicants before' AS TableName, COUNT(*) AS Rows FROM dbo.Applicants
UNION ALL
SELECT 'OtpTokens before', COUNT(*) FROM dbo.OtpTokens
UNION ALL
SELECT 'AuditLog before', COUNT(*) FROM dbo.AuditLog
UNION ALL
SELECT 'LoginAttempts before', COUNT(*) FROM dbo.LoginAttempts
UNION ALL
SELECT 'Applications before', COUNT(*) FROM dbo.Applications;
GO

-- Delete in dependency order (child tables first)
DELETE FROM dbo.Applications WHERE ApplicantId = 1;
DELETE FROM dbo.OtpTokens    WHERE ApplicantId = 1;
DELETE FROM dbo.AuditLog     WHERE ApplicantId = 1;
DELETE FROM dbo.Applicants   WHERE Id = 1;
GO

-- Show what's there now
SELECT 'Applicants after' AS TableName, COUNT(*) AS Rows FROM dbo.Applicants
UNION ALL
SELECT 'OtpTokens after', COUNT(*) FROM dbo.OtpTokens;
GO