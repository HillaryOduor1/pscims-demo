using System;
using System.Web;

namespace PSCIMS.Shared
{
    public static class RoleGuard
    {
        public static bool Require(HttpContext ctx, params string[] allowedRoles)
        {
            if (ctx.User == null || !ctx.User.Identity.IsAuthenticated)
            {
                ctx.Response.Redirect("~/jobs/loginPage.aspx");
                return false;
            }

            string role = ctx.Session["ApplicantRole"] as string;
            if (role == null) role = "applicant";

            foreach (var r in allowedRoles)
            {
                if (string.Equals(r, role, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            ctx.Response.StatusCode = 403;
            ctx.Response.Redirect("~/jobs/Error.aspx?code=403");
            return false;
        }

        public static int CurrentApplicantId(HttpContext ctx)
        {
            var o = ctx.Session["ApplicantId"];
            return o == null ? 0 : (int)o;
        }
    }
}
