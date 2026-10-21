using System;
using System.Security.Cryptography;

namespace PSCIMS.Shared
{
    public static class PasswordHasher
    {
        private const int SaltBytes = 16;
        private const int HashBytes = 32;
        private const int Iterations = 100000;

        public static void CreateHash(string password, out string hash, out string salt)
        {
            byte[] saltBytes = new byte[SaltBytes];
            using (var rng = new RNGCryptoServiceProvider())
                rng.GetBytes(saltBytes);

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, Iterations))
            {
                hash = Convert.ToBase64String(pbkdf2.GetBytes(HashBytes));
            }
            salt = Convert.ToBase64String(saltBytes);
        }

        public static bool Verify(string password, string storedHash, string storedSalt)
        {
            if (string.IsNullOrEmpty(password) ||
                string.IsNullOrEmpty(storedHash) ||
                string.IsNullOrEmpty(storedSalt))
                return false;

            byte[] saltBytes;
            byte[] storedHashBytes;
            try
            {
                saltBytes = Convert.FromBase64String(storedSalt);
                storedHashBytes = Convert.FromBase64String(storedHash);
            }
            catch { return false; }

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, Iterations))
            {
                byte[] computed = pbkdf2.GetBytes(HashBytes);
                if (computed.Length != storedHashBytes.Length) return false;

                int diff = 0;
                for (int i = 0; i < computed.Length; i++)
                    diff |= computed[i] ^ storedHashBytes[i];
                return diff == 0;
            }
        }

        public static string GenerateOtp()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] bytes = new byte[4];
                rng.GetBytes(bytes);
                uint value = BitConverter.ToUInt32(bytes, 0) % 1000000u;
                return value.ToString("D6");
            }
        }
    }
}
