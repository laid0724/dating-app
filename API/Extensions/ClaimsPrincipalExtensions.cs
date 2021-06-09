using System.Security.Claims;

namespace API.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        
        public static string GetUsername(this ClaimsPrincipal user)
        {
            /*
                we can access something called Claims from an http request, where we can extract and read the current
                user sending the request as a ClaimsPrincipal, to read more, see:
                https://docs.microsoft.com/en-us/dotnet/api/system.security.claims.claimsprincipal?view=net-5.0
            */
            return user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}