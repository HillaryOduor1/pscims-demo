using System;
using System.Data.SqlClient;
using System.IO;
using System.Web;

namespace PSCIMS.Shared
{
    public static class AuditLogger
    {
        private const string LogDir  = @"C:\temp1";
        private const string LogFile = @"C:\temp1\pscims_audit.log";
        private const string ErrFile = @"C:\temp1\pscims_audit_error.log";

        public static void Log(string eventType, int? applicantId, bool success, string detail = null)
        {
            try
            {
                EnsureLogDir();

                string line = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                            + " | " + eventType
                            + " | Id=" + (applicantId.HasValue ? applicantId.Value.ToString() : "-")
                            + " | Success=" + (success ? "Y" : "N")
                            + " | " + (detail ?? "")
                            + "\r\n";

                File.AppendAllText(LogFile, line);
            }
            catch (Exception ex)
            {
                TryWriteError(ex);
            }

            try
            {
                using (var conn = new SqlConnection(Config.ConnectionString))
                using (var cmd = new SqlCommand(
                    "INSERT INTO dbo.AuditLog (EventType, ApplicantId, IpAddress, UserAgent, Detail, Success) " +
                    "VALUES (@E, @A, @I, @U, @D, @S)", conn))
                {
                    cmd.Parameters.AddWithValue("@E", eventType ?? "");
                    cmd.Parameters.AddWithValue("@A", (object)applicantId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@I", (object)GetIp() ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@U", (object)Truncate(GetUserAgent(), 500) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@D", (object)Truncate(detail, 500) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@S", success);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception dbEx)
            {
                try
                {
                    EnsureLogDir();
                    File.AppendAllText(ErrFile,
                        DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                        + " | AuditDbFail | " + dbEx.GetType().Name + ": " + dbEx.Message + "\r\n");
                }
                catch { }
            }
        }

        private static void EnsureLogDir()
        {
            try
            {
                if (!Directory.Exists(LogDir))
                    Directory.CreateDirectory(LogDir);
            }
            catch { }
        }

        private static void TryWriteError(Exception ex)
        {
            try
            {
                EnsureLogDir();
                File.AppendAllText(ErrFile,
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    + " | " + ex.GetType().Name + ": " + ex.Message + "\r\n");
            }
            catch { }
        }

        private static string GetIp()
        {
            var ctx = HttpContext.Current;
            if (ctx == null) return null;
            string ip = ctx.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrWhiteSpace(ip)) ip = ctx.Request.UserHostAddress;
            if (!string.IsNullOrWhiteSpace(ip) && ip.Contains(","))
                ip = ip.Split(',')[0].Trim();
            return ip;
        }

        private static string GetUserAgent()
        {
            var ctx = HttpContext.Current;
            return ctx != null && ctx.Request != null ? ctx.Request.UserAgent : null;
        }

        private static string Truncate(string s, int max)
        {
            if (string.IsNullOrEmpty(s)) return s;
            return s.Length <= max ? s : s.Substring(0, max);
        }
    }
}

/*using System;
using System.Data.SqlClient;
using System.IO;
using System.Web;

namespace PSCIMS.Shared
{
    public static class AuditLogger
    {
        private const string LogFile = @"C:\temp1\pscims_audit.log";

        public static void Log(string eventType, int? applicantId, bool success, string detail = null)
        {
            try
            {
                string line = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                            + " | " + eventType
                            + " | Id=" + (applicantId.HasValue ? applicantId.Value.ToString() : "-")
                            + " | Success=" + (success ? "Y" : "N")
                            + " | " + (detail ?? "")
                            + "\r\n";

                File.AppendAllText(LogFile, line);
            }
            catch (Exception ex)
            {
                try
                {
                    File.AppendAllText(@"C:\temp1\log_exception.log",
                        DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " | " + ex.GetType().Name + ": " + ex.Message + "\r\n");
                }
                catch { }
            }

            try
            {
                using (var conn = new SqlConnection(Config.ConnectionString))
                using (var cmd = new SqlCommand(
                    "INSERT INTO dbo.AuditLog (EventType, ApplicantId, IpAddress, UserAgent, Detail, Success) " +
                    "VALUES (@E, @A, @I, @U, @D, @S)", conn))
                {
                    cmd.Parameters.AddWithValue("@E", eventType);
                    cmd.Parameters.AddWithValue("@A", (object)applicantId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@I", (object)GetIp() ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@U", (object)Truncate(GetUserAgent(), 500) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@D", (object)Truncate(detail, 500) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@S", success);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception dbEx)
            {
                try
                {
                    File.AppendAllText(LogFile,
                        DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " | AuditDbFail | " + dbEx.Message + "\r\n");
                }
                catch { }
            }
        }

        private static string GetIp()
        {
            var ctx = HttpContext.Current;
            if (ctx == null) return null;
            string ip = ctx.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrWhiteSpace(ip)) ip = ctx.Request.UserHostAddress;
            if (!string.IsNullOrWhiteSpace(ip) && ip.Contains(","))
                ip = ip.Split(',')[0].Trim();
            return ip;
        }

        private static string GetUserAgent()
        {
            var ctx = HttpContext.Current;
            return ctx != null && ctx.Request != null ? ctx.Request.UserAgent : null;
        }

        private static string Truncate(string s, int max)
        {
            if (string.IsNullOrEmpty(s)) return s;
            return s.Length <= max ? s : s.Substring(0, max);
        }
    }
}*/

/*using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web;

namespace PSCIMS.Shared
{
    public static class AuditLogger
    {
        /*private static string ConnStr
        {
            get { return ConfigurationManager.ConnectionStrings["PSCIMS"].ConnectionString; }
        }/
        private static string ConnStr
        {
            get { return Config.ConnectionString; }
        }

        public static void Log(string eventType, int? applicantId, bool success, string detail = null)
        {
            try
            {
                using (var conn = new SqlConnection(ConnStr))
                using (var cmd = new SqlCommand(
                    "INSERT INTO dbo.AuditLog (EventType, ApplicantId, IpAddress, UserAgent, Detail, Success) " +
                    "VALUES (@E, @A, @I, @U, @D, @S)", conn))
                {
                    cmd.Parameters.AddWithValue("@E", eventType);
                    cmd.Parameters.AddWithValue("@A", (object)applicantId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@I", (object)GetIp() ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@U", (object)Truncate(HttpContext.Current != null && HttpContext.Current.Request != null ? HttpContext.Current.Request.UserAgent : null, 500) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@D", (object)Truncate(detail, 500) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@S", success);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch
            {
                // Never let audit failure break the app
            }
        }

        private static string GetIp()
        {
            var ctx = HttpContext.Current;
            if (ctx == null) return null;
            string ip = ctx.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrWhiteSpace(ip)) ip = ctx.Request.UserHostAddress;
            if (!string.IsNullOrWhiteSpace(ip) && ip.Contains(","))
                ip = ip.Split(',')[0].Trim();
            return ip;
        }

        private static string Truncate(string s, int max)
        {
            if (string.IsNullOrEmpty(s)) return s;
            return s.Length <= max ? s : s.Substring(0, max);
        }
    }
}*/ 
 
 
 

