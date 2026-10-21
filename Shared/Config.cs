using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Net.Sockets;
using System.Web;

namespace PSCIMS.Shared
{
    public static class Config
    {
        private const int ProbeTimeoutMs = 1500;
        private const string ItemsKey = "__psc_internet_ok";
        private const string ItemsTimeKey = "__psc_internet_time";
        private const int CacheSeconds = 30;

        /// <summary>
        /// Resolves the active connection string. 
        /// Now configured to ALWAYS use Azure.
        /// </summary>
        public static string ConnectionString
        {
            get
            {
                string azure = ConfigurationManager.AppSettings["PSCIMS_Azure"];
                if (!string.IsNullOrEmpty(azure))
                {
                    return azure;
                }

                // Fallback just in case the Azure string is completely missing from Web.config
                string local = ConfigurationManager.AppSettings["PSCIMS_LocalDB"];
                if (!string.IsNullOrEmpty(local))
                    return local;

                var legacy = ConfigurationManager.ConnectionStrings["PSCIMS"];
                if (legacy != null && !string.IsNullOrEmpty(legacy.ConnectionString))
                    return legacy.ConnectionString;

                throw new InvalidOperationException(
                    "No database connection string configured. Set PSCIMS_Azure in Web.config.local.");
            }
        }

        public static string WhichDatabase()
        {
            return "Azure";
        }

        /// <summary>
        /// Actively probes for internet. Null-safe with respect to HttpContext
        /// and Session. Uses HttpContext.Items for per-request caching.
        /// </summary>
        public static bool HasInternet()
        {
            HttpContext ctx = null;
            try { ctx = HttpContext.Current; } catch { }

            // Per-request cache via Items (always available when ctx != null)
            if (ctx != null && ctx.Items != null)
            {
                try
                {
                    var cachedTime = ctx.Items[ItemsTimeKey] as DateTime?;
                    var cachedValue = ctx.Items[ItemsKey] as bool?;

                    if (cachedTime.HasValue && cachedValue.HasValue)
                    {
                        if ((DateTime.UtcNow - cachedTime.Value).TotalSeconds < CacheSeconds)
                            return cachedValue.Value;
                    }
                }
                catch { }
            }

            // Do the probe
            bool ok;
            try
            {
                ok = ProbeInternet();
            }
            catch
            {
                ok = false;
            }

            // Cache it in Items for the rest of this request
            if (ctx != null && ctx.Items != null)
            {
                try
                {
                    ctx.Items[ItemsTimeKey] = DateTime.UtcNow;
                    ctx.Items[ItemsKey] = ok;
                }
                catch { }
            }

            return ok;
        }

        /// <summary>
        /// TCP probe to well-known DNS servers. Fast (~1.5s max).
        /// </summary>
        private static bool ProbeInternet()
        {
            string[] hosts = { "208.67.222.222", "8.8.8.8" };

            foreach (var host in hosts)
            {
                try
                {
                    using (var client = new TcpClient())
                    {
                        var result = client.BeginConnect(host, 53, null, null);
                        bool success = result.AsyncWaitHandle.WaitOne(ProbeTimeoutMs);

                        if (success && client.Connected)
                        {
                            client.EndConnect(result);
                            return true;
                        }
                    }
                }
                catch
                {
                    // try next host
                }
            }

            return false;
        }

        private static string GetTarget()
        {
            string t = ConfigurationManager.AppSettings["DatabaseTarget"];
            return string.IsNullOrEmpty(t) ? "Azure" : t.Trim();
        }
    }
}

/*using System;
using System.Configuration;
using System.Net.Sockets;
using System.Web;

namespace PSCIMS.Shared
{
    public static class Config
    {
        private const int ProbeTimeoutMs = 1500;
        private const string ItemsKey = "__psc_internet_ok";
        private const string ItemsTimeKey = "__psc_internet_time";
        private const int CacheSeconds = 30;

        /// <summary>
        /// Resolves the active connection string based on DatabaseTarget
        /// and current internet connectivity.
        /// </summary>
        public static string ConnectionString
        {
            get
            {
                string target = GetTarget();

                if (string.Equals(target, "Azure", StringComparison.OrdinalIgnoreCase))
                {
                    if (HasInternet())
                    {
                        string azure = ConfigurationManager.AppSettings["PSCIMS_Azure"];
                        if (!string.IsNullOrEmpty(azure))
                            return azure;
                    }
                    // Offline fallback
                }

                string local = ConfigurationManager.AppSettings["PSCIMS_LocalDB"];
                if (!string.IsNullOrEmpty(local))
                    return local;

                var legacy = ConfigurationManager.ConnectionStrings["PSCIMS"];
                if (legacy != null && !string.IsNullOrEmpty(legacy.ConnectionString))
                    return legacy.ConnectionString;

                throw new InvalidOperationException(
                    "No database connection string configured. Set PSCIMS_LocalDB " +
                    "or PSCIMS_Azure in Web.config / Web.config.local.");
            }
        }

        public static string WhichDatabase()
        {
            string target = GetTarget();
            bool online = HasInternet();

            if (string.Equals(target, "Azure", StringComparison.OrdinalIgnoreCase))
            {
                if (online) return "Azure";
                return "LocalDB (Azure configured, offline fallback)";
            }

            return "LocalDB";
        }

        /// <summary>
        /// Actively probes for internet. Null-safe with respect to HttpContext
        /// and Session. Uses HttpContext.Items for per-request caching.
        /// </summary>
        public static bool HasInternet()
        {
            HttpContext ctx = null;
            try { ctx = HttpContext.Current; } catch { }

            // Per-request cache via Items (always available when ctx != null)
            if (ctx != null && ctx.Items != null)
            {
                try
                {
                    var cachedTime = ctx.Items[ItemsTimeKey] as DateTime?;
                    var cachedValue = ctx.Items[ItemsKey] as bool?;

                    if (cachedTime.HasValue && cachedValue.HasValue)
                    {
                        if ((DateTime.UtcNow - cachedTime.Value).TotalSeconds < CacheSeconds)
                            return cachedValue.Value;
                    }
                }
                catch { }
            }

            // Do the probe
            bool ok;
            try
            {
                ok = ProbeInternet();
            }
            catch
            {
                ok = false;
            }

            // Cache it in Items for the rest of this request
            if (ctx != null && ctx.Items != null)
            {
                try
                {
                    ctx.Items[ItemsTimeKey] = DateTime.UtcNow;
                    ctx.Items[ItemsKey] = ok;
                }
                catch { }
            }

            return ok;
        }

        /// <summary>
        /// TCP probe to well-known DNS servers. Fast (~1.5s max), doesn't
        /// require a successful DNS lookup (avoids false negatives when
        /// DNS itself is what's broken).
        /// </summary>
        private static bool ProbeInternet()
        {
            // Port 53 is DNS. Two well-known public DNS servers.
            string[] hosts = { "208.67.222.222", "8.8.8.8" };

            foreach (var host in hosts)
            {
                try
                {
                    using (var client = new TcpClient())
                    {
                        var result = client.BeginConnect(host, 53, null, null);
                        bool success = result.AsyncWaitHandle.WaitOne(ProbeTimeoutMs);

                        if (success && client.Connected)
                        {
                            client.EndConnect(result);
                            return true;
                        }
                    }
                }
                catch
                {
                    // try next host
                }
            }

            return false;
        }

        private static string GetTarget()
        {
            string t = ConfigurationManager.AppSettings["DatabaseTarget"];
            return string.IsNullOrEmpty(t) ? "LocalDB" : t.Trim();
        }
    }
}*/