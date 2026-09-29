@echo off
cd /d C:\inetpub\wwwroot\PSCIMS

if not exist bin mkdir bin

C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe ^
  /target:library ^
  /out:bin\PSCIMS.dll ^
  /reference:System.dll ^
  /reference:System.Web.dll ^
  /reference:System.Data.dll ^
  /reference:System.Configuration.dll ^
  /reference:System.Web.Extensions.dll ^
  /reference:System.Core.dll ^
  /reference:System.Drawing.dll ^
  Site.Master.cs ^
  Site.Master.designer.cs ^
  jobs\Default.aspx.cs ^
  jobs\loginPage.aspx.cs ^
  jobs\loginPage.aspx.designer.cs ^
  jobs\RegisterProfile.aspx.cs ^
  jobs\RegisterProfile.aspx.designer.cs ^
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
  puio\Default.aspx.designer.cs

if %errorlevel% neq 0 (
  echo.
  echo *** BUILD FAILED ***
  pause
  exit /b 1
)

echo.
echo Build OK. Reloading IIS...
echo. >> Web.config

echo Done. Refresh your browser with Ctrl+F5.
pause