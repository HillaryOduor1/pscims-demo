using System;
using System.Text.RegularExpressions;
using System.Web;

namespace PSCIMS.Shared
{
    public static class InputSanitizer
    {
        // Kenyan national ID: 7-10 alphanumeric. Passport: alphanumeric, 7-10 chars.
        private static readonly Regex IdRegex    = new Regex(@"^[A-Za-z0-9]{7,10}$", RegexOptions.Compiled);
        private static readonly Regex EmailRegex = new Regex(@"^[^\s@]+@[^\s@]+\.[^\s@]{2,}$", RegexOptions.Compiled);
        private static readonly Regex NameRegex  = new Regex(@"^[\p{L}\s'\-\.]{2,100}$", RegexOptions.Compiled);

        // NO backslash before underscore, NO quote character (simplest safe set)
        private static readonly Regex SafeText   = new Regex(@"^[\p{L}\p{N}\s\.\,\-\?\!\&\(\)\/\:]+$", RegexOptions.Compiled);

        public static string Trim(string input)
        {
            return (input ?? "").Trim();
        }

        public static string Clean(string input, int maxLength = 200)
        {
            if (string.IsNullOrEmpty(input)) return "";
            string s = input.Trim();
            if (s.Length > maxLength) s = s.Substring(0, maxLength);
            return s;
        }

        public static bool IsValidId(string id)
        {
            return !string.IsNullOrEmpty(id) && IdRegex.IsMatch(id);
        }

        public static bool IsValidEmail(string email)
        {
            return !string.IsNullOrEmpty(email) && email.Length <= 200 && EmailRegex.IsMatch(email);
        }

        public static bool IsValidName(string name)
        {
            return !string.IsNullOrEmpty(name) && NameRegex.IsMatch(name);
        }

        public static bool IsSafeText(string s)
        {
            return string.IsNullOrEmpty(s) || SafeText.IsMatch(s);
        }

        public static string Enc(string s)
        {
            return HttpUtility.HtmlEncode(s ?? "");
        }

        public static string EncOneLine(string s, int max = 500)
        {
            if (string.IsNullOrEmpty(s)) return "";
            string t = s.Replace("\r", " ").Replace("\n", " ").Trim();
            if (t.Length > max) t = t.Substring(0, max);
            return HttpUtility.HtmlEncode(t);
        }
    }
}