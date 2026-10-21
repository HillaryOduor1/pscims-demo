using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace PSCIMS.Shared
{
    public static class ContentProvider
    {
        private static Dictionary<string, string> _cache = new Dictionary<string, string>();
        private static DateTime _cacheTime = DateTime.MinValue;

        public static string Get(string key, string fallback = "")
        {
            // Cache for 5 minutes to avoid hitting DB on every request
            if ((DateTime.Now - _cacheTime).TotalMinutes > 5)
            {
                LoadCache();
            }

            if (_cache.ContainsKey(key))
                return _cache[key];

            return fallback;
        }

        private static void LoadCache()
        {
            var tempCache = new Dictionary<string, string>();
            try
            {
                using (var conn = new SqlConnection(Config.ConnectionString))
                using (var cmd = new SqlCommand("SELECT ContentKey, ContentValue FROM dbo.SiteContent", conn))
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tempCache[reader.GetString(0)] = reader.GetString(1);
                        }
                    }
                }
                _cache = tempCache;
                _cacheTime = DateTime.Now;
            }
            catch
            {
                // If DB fails, keep the old cache
            }
        }
    }
}