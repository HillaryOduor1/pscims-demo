@echo off
cd /d C:\inetpub\wwwroot\PSCIMS

if not exist bin mkdir bin

echo.
echo === Building bin\PSCIMS.dll ===
echo.

C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe ^
  /target:library ^
  /out:bin\PSCIMS.dll ^
  /nologo ^
  /reference:System.dll ^
  /reference:System.Web.dll ^
  /reference:System.Data.dll ^
  /reference:System.Configuration.dll ^
  /reference:System.Web.Extensions.dll ^
  /reference:System.Core.dll ^
  /reference:System.Drawing.dll ^
  Site.Master.cs ^
  Site.Master.designer.cs ^
  Shared\AdvertDal.cs ^
  Shared\ApplicantDal.cs ^
  Shared\AuditLogger.cs ^
  Shared\Config.cs ^
  Shared\ContentProvider.cs ^
  Shared\CsrfProtector.cs ^
  Shared\EmailService.cs ^
  Shared\InputSanitizer.cs ^
  Shared\LoginThrottle.cs ^
  Shared\OtpService.cs ^
  Shared\PasswordHasher.cs ^
  Shared\RoleGuard.cs ^
  Shared\SessionContext.cs ^
  jobs\Default.aspx.cs ^
  jobs\Application.aspx.cs ^
  jobs\Application.aspx.designer.cs ^
  jobs\loginPage.aspx.cs ^
  jobs\loginPage.aspx.designer.cs ^
  jobs\ForgotPassword.aspx.cs ^
  jobs\ForgotPassword.aspx.designer.cs ^
  jobs\RegisterProfile.aspx.cs ^
  jobs\RegisterProfile.aspx.designer.cs ^
  jobs\VerifyOtp.aspx.cs ^
  jobs\VerifyOtp.aspx.designer.cs ^
  jobs\ApplicantDashboard.aspx.cs ^
  jobs\ApplicantDashboard.aspx.designer.cs ^
  jobs\logout.aspx.cs ^
  jobs\logout.aspx.designer.cs ^
  jobs\ActiveJobsAdverts.aspx.cs ^
  jobs\ActiveJobsAdverts.aspx.designer.cs ^
  jobs\ActiveAdvertsInternsInternshipExt.aspx.cs ^
  jobs\ActiveAdvertsInternsInternshipExt.aspx.designer.cs ^
  jobs\ELPEXT.aspx.cs ^
  jobs\ELPEXT.aspx.designer.cs ^
  jobs\AdvertStatusGlobal.aspx.cs ^
  jobs\AdvertStatusGlobal.aspx.designer.cs ^
  jobs\Error.aspx.cs ^
  jobs\Error.aspx.designer.cs ^
  jobs\ComingSoon.aspx.cs ^
  jobs\ComingSoon.aspx.designer.cs ^
  puio\Default.aspx.cs ^
  puio\Default.aspx.designer.cs ^
  Admin\Default.aspx.cs ^
  Admin\ManageAdverts.aspx.cs ^
  Admin\ManageAdverts.aspx.designer.cs ^
  Admin\ManageContent.aspx.cs ^
  Admin\ManageContent.aspx.designer.cs ^
  Admin\ManageUsers.aspx.cs ^
  Admin\ManageUsers.aspx.designer.cs ^
  Admin\ViewApplications.aspx.cs ^
  Admin\ViewApplications.aspx.designer.cs

if %errorlevel% neq 0 (
  echo.
  echo *** BUILD FAILED ***
  pause
  exit /b 1
)

echo.
echo Build OK.
echo Forcing IIS app domain reload...
echo. >> C:\inetpub\wwwroot\PSCIMS\Web.config

echo.
echo Done. Refresh your browser with Ctrl+F5.
pause