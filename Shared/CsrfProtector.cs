using System;
using System.Security.Cryptography;
using System.Web;

namespace PSCIMS.Shared
{
    public static class CsrfProtector
    {
        public const string CookieName = "__psc-csrf";
        public const string FieldName  = "__psc-csrf-field";

        public static string IssueToken(HttpContext ctx)
        {
            string token = RequestToken();
            ctx.Response.Cookies[CookieName].Value = token;
            ctx.Response.Cookies[CookieName].HttpOnly = true;
            ctx.Response.Cookies[CookieName].Secure = ctx.Request.IsSecureConnection;
            ctx.Response.Cookies[CookieName].SameSite = SameSiteMode.Strict;
            return token;
        }

        public static bool Validate(HttpContext ctx, string submitted)
        {
            var cookie = ctx.Request.Cookies[CookieName];
            if (cookie == null || string.IsNullOrEmpty(cookie.Value)) return false;
            if (string.IsNullOrEmpty(submitted)) return false;
            return SlowEquals(cookie.Value, submitted);
        }

        private static string RequestToken()
        {
            byte[] bytes = new byte[32];
            using (var rng = new RNGCryptoServiceProvider())
                rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        private static bool SlowEquals(string a, string b)
        {
            if (a == null || b == null || a.Length != b.Length) return false;
            int diff = 0;
            for (int i = 0; i < a.Length; i++)
                diff |= a[i] ^ b[i];
            return diff == 0;
        }
    }
}
