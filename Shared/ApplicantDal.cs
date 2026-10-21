using System;
using System.Configuration;
using System.Data.SqlClient;

namespace PSCIMS.Shared
{
    public static class ApplicantDal
    {
        private static string ConnStr
        {
            get { return Config.ConnectionString; }
        }

        public static bool Exists(string idNumber, string email)
        {
            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(
                "SELECT COUNT(*) FROM dbo.Applicants WHERE IdNumber = @Id OR Email = @Email", conn))
            {
                cmd.Parameters.AddWithValue("@Id", idNumber);
                cmd.Parameters.AddWithValue("@Email", email);
                conn.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        public static int Register(string idNumber, string surname, string firstName,
                                   string otherNames, string email, string password)
        {
            string hash, salt;
            PasswordHasher.CreateHash(password, out hash, out salt);

            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(
                "INSERT INTO dbo.Applicants " +
                "(IdNumber, Surname, FirstName, OtherNames, Email, PasswordHash, PasswordSalt, RoleId) " +
                "OUTPUT INSERTED.Id " +
                "VALUES (@Id, @Sn, @Fn, @On, @Em, @Hash, @Salt, 1)", conn))
            {
                cmd.Parameters.AddWithValue("@Id",   idNumber);
                cmd.Parameters.AddWithValue("@Sn",   surname);
                cmd.Parameters.AddWithValue("@Fn",   firstName);
                cmd.Parameters.AddWithValue("@On",   (object)otherNames ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Em",   email);
                cmd.Parameters.AddWithValue("@Hash", hash);
                cmd.Parameters.AddWithValue("@Salt", salt);

                conn.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

        public class AuthResult
        {
            public int Id { get; set; }
            public string Role { get; set; }
            public bool EmailVerified { get; set; }
        }

        public static AuthResult Validate(string idNumber, string password)
        {
            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(
                "SELECT a.Id, a.PasswordHash, a.PasswordSalt, a.EmailVerified, r.Name AS Role " +
                "FROM dbo.Applicants a " +
                "LEFT JOIN dbo.Roles r ON r.Id = a.RoleId " +
                "WHERE a.IdNumber = @Id AND a.IsActive = 1", conn))
            {
                cmd.Parameters.AddWithValue("@Id", idNumber);
                conn.Open();

                using (var r = cmd.ExecuteReader())
                {
                    if (!r.Read()) return null;

                    string hash = r["PasswordHash"].ToString();
                    string salt = r["PasswordSalt"].ToString();

                    if (!PasswordHasher.Verify(password, hash, salt))
                        return null;

                    return new AuthResult
                    {
                        Id = (int)r["Id"],
                        Role = r["Role"] != DBNull.Value ? r["Role"].ToString() : "applicant",
                        EmailVerified = (bool)r["EmailVerified"]
                    };
                }
            }
        }

        public static void RecordLogin(int applicantId)
        {
            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(
                "UPDATE dbo.Applicants SET LastLoginAt = SYSUTCDATETIME() WHERE Id = @Id", conn))
            {
                cmd.Parameters.AddWithValue("@Id", applicantId);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public static void MarkEmailVerified(int applicantId)
        {
            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(
                "UPDATE dbo.Applicants SET EmailVerified = 1 WHERE Id = @Id", conn))
            {
                cmd.Parameters.AddWithValue("@Id", applicantId);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public static string GetEmail(int applicantId)
        {
            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(
                "SELECT Email FROM dbo.Applicants WHERE Id = @Id", conn))
            {
                cmd.Parameters.AddWithValue("@Id", applicantId);
                conn.Open();
                var o = cmd.ExecuteScalar();
                return o == null ? null : o.ToString();
            }
        }

        public static string GetFirstName(int applicantId)
        {
            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(
                "SELECT FirstName FROM dbo.Applicants WHERE Id = @Id", conn))
            {
                cmd.Parameters.AddWithValue("@Id", applicantId);
                conn.Open();
                var o = cmd.ExecuteScalar();
                return o == null ? null : o.ToString();
            }
        }
    }
}