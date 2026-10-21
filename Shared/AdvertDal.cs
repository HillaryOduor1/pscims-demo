using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Text;

namespace PSCIMS.Shared
{
    public class Advert
    {
        public int Id { get; set; }
        public string Reference { get; set; }
        public string Title { get; set; }
        public string Ministry { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public string Kind { get; set; }
        public DateTime OpensOn { get; set; }
        public DateTime ClosesOn { get; set; }

        // C# 5 compatible â€” no expression-bodied property
        public bool IsOpen
        {
            get { return DateTime.Today <= ClosesOn.Date; }
        }
    }

    public static class AdvertDal
    {
        /*private static string ConnStr
        {
            get { return ConfigurationManager.ConnectionStrings["PSCIMS"].ConnectionString; }
        }*/
        private static string ConnStr
        {
            get { return Config.ConnectionString; }
        }

        public static List<Advert> GetActiveAdverts(string kind = null, string search = null)
        {
            var list = new List<Advert>();

            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand())
            {
                cmd.Connection = conn;

                var sql = new StringBuilder(
                    "SELECT Id, Reference, Title, Ministry, Location, Description, Kind, OpensOn, ClosesOn " +
                    "FROM dbo.Adverts WHERE 1 = 1");

                if (!string.IsNullOrEmpty(kind))
                {
                    sql.Append(" AND Kind = @Kind");
                    cmd.Parameters.AddWithValue("@Kind", kind);
                }

                if (!string.IsNullOrEmpty(search))
                {
                    sql.Append(" AND (Title LIKE @Q OR Reference LIKE @Q OR Ministry LIKE @Q)");
                    cmd.Parameters.AddWithValue("@Q", "%" + search.Replace("%", "[%]").Replace("_", "[_]") + "%");
                }

                sql.Append(" ORDER BY ClosesOn");
                cmd.CommandText = sql.ToString();

                conn.Open();
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        list.Add(new Advert
                        {
                            Id          = (int)r["Id"],
                            Reference   = r["Reference"].ToString(),
                            Title       = r["Title"].ToString(),
                            Ministry    = r["Ministry"].ToString(),
                            Location    = r["Location"].ToString(),
                            Description = r["Description"] == DBNull.Value ? "" : r["Description"].ToString(),
                            Kind        = r["Kind"].ToString(),
                            OpensOn     = Convert.ToDateTime(r["OpensOn"]),
                            ClosesOn    = Convert.ToDateTime(r["ClosesOn"])
                        });
                    }
                }
            }
            return list;
        }
    }
}
