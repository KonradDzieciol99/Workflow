using System.Security.Claims;

namespace WorkflowApi.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetUserNameEmail(this ClaimsPrincipal claimsPrincipal)
        {
            string name_email =claimsPrincipal.FindFirst(ClaimTypes.Name)?.Value;

            return name_email;
        }

        public static int GetUserId(this ClaimsPrincipal claimsPrincipal)
        {
            int id = int.Parse(claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            return id;
        }

    }
}
