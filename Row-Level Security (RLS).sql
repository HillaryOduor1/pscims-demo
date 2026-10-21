USE [pscims-db];
GO

-- Session context — app sets this on every connection
CREATE OR ALTER PROCEDURE sp_set_applicant_context
    @ApplicantId INT
AS
BEGIN
    SET NOCOUNT ON;
    EXEC sp_set_session_context @key = N'ApplicantId', @value = @ApplicantId;
END
GO

-- Predicate function: filter Applications to current user
CREATE OR ALTER FUNCTION dbo.fn_application_access(@ApplicantId INT)
RETURNS TABLE
WITH SCHEMABINDING
AS
RETURN
    SELECT 1 AS ok
    WHERE
        -- Admin / reviewer sees all
        CAST(SESSION_CONTEXT(N'ApplicantId') AS INT) = -1
        -- Applicant sees only own rows
        OR @ApplicantId = CAST(SESSION_CONTEXT(N'ApplicantId') AS INT);
GO

-- Security policy
CREATE SECURITY POLICY dbo.ApplicationSecurityPolicy
    ADD FILTER PREDICATE dbo.fn_application_access(ApplicantId) ON dbo.Applications,
    ADD BLOCK  PREDICATE dbo.fn_application_access(ApplicantId) ON dbo.Applications AFTER INSERT
WITH (STATE = ON);
GO