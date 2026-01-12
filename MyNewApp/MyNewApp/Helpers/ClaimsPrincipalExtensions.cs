using System.Security.Claims;

namespace MyNewApp.Helpers
{
    public static class ClaimsPrincipalExtensions
    {
        public static long GetUserId(this ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            return long.Parse(userId!);
        }
    }
}
