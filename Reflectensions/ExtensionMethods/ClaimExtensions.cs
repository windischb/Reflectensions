using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Reflectensions.ExtensionMethods
{
    public static class ClaimExtensions
    {

        public static IEnumerable<Claim> GetClaimsByType(IEnumerable<Claim> claims, string type) => ClaimHelpers.GetClaimsByType(claims, type);
        public static Claim GetFirstClaimByType(IEnumerable<Claim> claims, string type) => ClaimHelpers.GetFirstClaimByType(claims, type);
        public static string GetFirstClaimValueByType(IEnumerable<Claim> claims, string type) => ClaimHelpers.GetFirstClaimValueByType(claims, type);
        public static IEnumerable<string> GetClaimValuesByType(IEnumerable<Claim> claims, string type) => ClaimHelpers.GetClaimValuesByType(claims, type);
        public static List<Claim> RemoveClaimsByType(List<Claim> claims, string type) => ClaimHelpers.RemoveClaimsByType(claims, type);

    }
}
