using System.Security.Claims;

namespace API.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        /*
            we can access something called Claims from an http request, where we can extract and read the current
            user sending the request as a ClaimsPrincipal, to read more, see:
            https://docs.microsoft.com/en-us/dotnet/api/system.security.claims.claimsprincipal?view=net-5.0
        */
        public static string GetUsername(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Name)?.Value;
        }
        public static int GetUserId(this ClaimsPrincipal user)
        {
            return int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }
    }
}