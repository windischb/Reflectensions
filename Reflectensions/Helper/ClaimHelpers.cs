using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Reflectensions.Helper
{
    public static class ClaimHelpers
    {

        public static IEnumerable<Claim> GetClaimsByType(IEnumerable<Claim> claims, string type) {
            return claims.Where(c => c.Type.Equals(type, StringComparison.CurrentCultureIgnoreCase));
        }

        public static Claim GetFirstClaimByType(IEnumerable<Claim> claims, string type)
        {
            return claims.FirstOrDefault(c => c.Type.Equals(type, StringComparison.CurrentCultureIgnoreCase));
        }

        public static string GetFirstClaimValueByType(IEnumerable<Claim> claims, string type)
        {
            return GetFirstClaimByType(claims, type)?.Value;
        }

        public static IEnumerable<string> GetClaimValuesByType(IEnumerable<Claim> claims, string type) {
            return GetClaimsByType(claims, type)?.Select(c => c.Value);
        }

        public static List<Claim> RemoveClaimsByType(List<Claim> claims, string type)
        {
            claims.RemoveAll(c => c.Type.Equals(type, StringComparison.CurrentCultureIgnoreCase));
            return claims;
        }
    }
}
