using System;
using System.Configuration;
using System.Data.SqlClient;

namespace PSCIMS.Shared
{
    public static class OtpService
    {
        private static string ConnStr
        {
            get { return Config.ConnectionString; }
        }

        private const int TtlMinutes = 10;

        // Creates a new OTP, invalidating any previous ones for the same purpose.
        public static string Create(int applicantId, string purpose)
        {
            string code = PasswordHasher.GenerateOtp();

            using (var conn = new SqlConnection(ConnStr))
            {
                conn.Open();

                // Invalidate prior unconsumed tokens (separate command, no transaction)
                using (var cmd = new SqlCommand(
                    "UPDATE dbo.OtpTokens SET ConsumedAt = SYSUTCDATETIME() " +
                    "WHERE ApplicantId = @A AND Purpose = @P AND ConsumedAt IS NULL", conn))
                {
                    cmd.Parameters.AddWithValue("@A", applicantId);
                    cmd.Parameters.AddWithValue("@P", purpose);
                    cmd.ExecuteNonQuery();
                }

                // Insert the new one
                using (var cmd = new SqlCommand(
                    "INSERT INTO dbo.OtpTokens (ApplicantId, Code, Purpose, ExpiresAt) " +
                    "VALUES (@A, @C, @P, DATEADD(MINUTE, @T, SYSUTCDATETIME()))", conn))
                {
                    cmd.Parameters.AddWithValue("@A", applicantId);
                    cmd.Parameters.AddWithValue("@C", code);
                    cmd.Parameters.AddWithValue("@P", purpose);
                    cmd.Parameters.AddWithValue("@T", TtlMinutes);
                    cmd.ExecuteNonQuery();
                }
            }

            return code;
        }

        // Verifies an OTP. Returns true and consumes the token on success.
        public static bool Verify(int applicantId, string code, string purpose)
        {
            if (string.IsNullOrEmpty(code) || code.Length != 6) return false;

            using (var conn = new SqlConnection(ConnStr))
            {
                conn.Open();

                using (var cmd = new SqlCommand(
                    "UPDATE dbo.OtpTokens SET ConsumedAt = SYSUTCDATETIME() " +
                    "OUTPUT INSERTED.Id " +
                    "WHERE ApplicantId = @A AND Purpose = @P AND Code = @C " +
                    "  AND ConsumedAt IS NULL AND ExpiresAt > SYSUTCDATETIME()", conn))
                {
                    cmd.Parameters.AddWithValue("@A", applicantId);
                    cmd.Parameters.AddWithValue("@C", code);
                    cmd.Parameters.AddWithValue("@P", purpose);

                    var id = cmd.ExecuteScalar();
                    return id != null;
                }
            }
        }
    }
}