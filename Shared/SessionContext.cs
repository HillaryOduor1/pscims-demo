using System.Data.SqlClient;
using System.Web;

namespace PSCIMS.Shared
{
    public static class SessionContext
    {
        public static void Apply(SqlConnection openConnection)
        {
            var ctx = HttpContext.Current;
            if (ctx == null || ctx.User == null || !ctx.User.Identity.IsAuthenticated)
            {
                Exec(openConnection, -1);
                return;
            }

            var id = ctx.Session["ApplicantId"];
            if (id == null)
            {
                Exec(openConnection, -1);
                return;
            }

            Exec(openConnection, (int)id);
        }

        private static void Exec(SqlConnection conn, int applicantId)
        {
            using (var cmd = new SqlCommand("EXEC sp_set_applicant_context @Id", conn))
            {
                cmd.Parameters.AddWithValue("@Id", applicantId);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
