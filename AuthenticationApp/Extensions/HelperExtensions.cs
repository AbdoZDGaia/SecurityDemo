using System.Security.Claims;

namespace AuthenticationApp.Extensions
{
    public static class HelperExtensions
    {
        public static string GetClaim(this List<Claim> claims, string type)
        {
            return claims.FirstOrDefault(claims => claims.Type == type)?.Value;
        }
    }
}
