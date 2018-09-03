using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Reflectensions.Helpers
{

    public class Wildcard : Regex {

        public static List<string> WildcardCharacters = new List<string> { "*", "?", "+" };

        public Wildcard(string pattern)
            : base(WildcardToRegex(pattern)) {
        }

        public Wildcard(string pattern, RegexOptions options)
            : base(WildcardToRegex(pattern), options) {
        }

        public static string WildcardToRegex(string pattern) {
            return "^" + Escape(pattern).
                       Replace("\\*", ".*").
                       Replace("\\?", ".").
                       Replace("\\+", ".+") + "$";
        }

        public static Boolean IsMatch(String searchIn, String matchString, bool ignoreCase = true, Boolean invert = false) {
            if (searchIn == null)
                searchIn = "";

            RegexOptions regOpts = RegexOptions.None;
            if (ignoreCase)
                regOpts = regOpts | RegexOptions.IgnoreCase;

            var rege = new Wildcard(matchString, regOpts).IsMatch(searchIn);

            if (invert)
                return !rege;

            return rege;
        }

        public static bool ContainsWildcard(string value) {

            return WildcardCharacters.Any(value.Contains);
        }
    }
}
