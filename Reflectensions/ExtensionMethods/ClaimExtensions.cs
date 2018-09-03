using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Reflectensions.ExtensionMethods
{
    public static class ClaimExtensions
    {

        public static IEnumerable<Claim> GetClaimsByType(this IEnumerable<Claim> claims, string type) {
            return claims.Where(c => c.Type.Equals(type, StringComparison.CurrentCultureIgnoreCase));
        }

        public static Claim GetFirstClaimByType(this IEnumerable<Claim> claims, string type)
        {
            return claims.FirstOrDefault(c => c.Type.Equals(type, StringComparison.CurrentCultureIgnoreCase));
        }

        public static string GetFirstClaimValueByType(this IEnumerable<Claim> claims, string type)
        {
            return claims.GetFirstClaimByType(type)?.Value;
        }

        public static List<Claim> RemoveClaimsByType(this List<Claim> claims, string type)
        {
            claims.RemoveAll(c => c.Type.Equals(type, StringComparison.CurrentCultureIgnoreCase));
            return claims;
        }
    }
}
