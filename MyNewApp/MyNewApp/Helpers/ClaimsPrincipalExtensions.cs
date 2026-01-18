using System.Security;
using System.Security.Claims;

namespace MyNewApp.Helpers
{
    public static class ClaimsPrincipalExtensions
    {
        public static long GetUserId(this ClaimsPrincipal user)
        {
            if (user?.Identity?.IsAuthenticated != true)
                throw new SecurityException("User is not authenticated.");

            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            return long.Parse(userId!);
        }

        public static (long UserId, string UserName) GetUserIdAndUserName(
            this ClaimsPrincipal user)
        {
            if (user?.Identity?.IsAuthenticated != true)
                throw new SecurityException("User is not authenticated.");

            var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = user.FindFirstValue(ClaimTypes.Name);

            if (!long.TryParse(userIdClaim, out var userId))
                throw new SecurityException("Invalid or missing user id claim.");

            if (string.IsNullOrWhiteSpace(userName))
                throw new SecurityException("Invalid or missing username claim.");

            return (userId, userName);
        }
    }
}
