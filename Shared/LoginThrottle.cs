using System;
using System.Configuration;
using System.Data.SqlClient;

namespace PSCIMS.Shared
{
    public static class LoginThrottle
    {
        /*private static string ConnStr
        {
            get { return ConfigurationManager.ConnectionStrings["PSCIMS"].ConnectionString; }
        }*/
        private static string ConnStr
        {
            get { return Config.ConnectionString; }
        }

        private const int MaxAttempts = 5;
        private const int WindowMinutes = 15;

        public static bool IsThrottled(string identifier, string kind)
        {
            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(
                "SELECT COUNT(*) FROM dbo.LoginAttempts " +
                "WHERE Identifier = @I AND Kind = @K AND Success = 0 " +
                "  AND AttemptedAt > DATEADD(MINUTE, -@W, SYSUTCDATETIME())", conn))
            {
                cmd.Parameters.AddWithValue("@I", identifier ?? "");
                cmd.Parameters.AddWithValue("@K", kind);
                cmd.Parameters.AddWithValue("@W", WindowMinutes);
                conn.Open();
                return (int)cmd.ExecuteScalar() >= MaxAttempts;
            }
        }

        public static void Record(string identifier, string kind, bool success)
        {
            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(
                "INSERT INTO dbo.LoginAttempts (Identifier, Kind, Success) VALUES (@I, @K, @S)", conn))
            {
                cmd.Parameters.AddWithValue("@I", identifier ?? "");
                cmd.Parameters.AddWithValue("@K", kind);
                cmd.Parameters.AddWithValue("@S", success);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
