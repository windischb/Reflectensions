using System.Linq;
using System.Text.RegularExpressions;

namespace Reflectensions
{
    public static class StringExtensions
    {
        public static string WildcardToRegex(this string pattern)
        {
            return "^" + Regex.Escape(pattern)
                       .Replace("\\*", "(.*)")
                       .Replace("\\+", "(.+)")
                       .Replace("\\?", "(.)") + "$";
        }

        public static StringDifferences Differences(this string value, string pattern, bool ignoreCase = true)
        {

            var rating = new StringDifferences();
            if (value == null)
                value = "";

            var options = ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None;

            var matches = new Regex(WildcardToRegex(pattern), options).Matches(value);

            if (matches == null || matches.Count == 0)
            {
                return rating.SetFailed();
            };

            foreach (Match match in matches)
            {
                for (int i = 1; i < match.Groups.Count; i++)
                {
                    rating.AddMatch(match.Groups[i].Index, match.Groups[i].Length, match.Groups[i].Value);
                }

            }

            return rating;
        }

        public static int DifferencesCount(this string value, string pattern)
        {

            if (string.IsNullOrWhiteSpace(value))
                return -1;

            var ignoreCase = pattern.StartsWith("@");
            pattern = pattern.TrimStart('@');
            var rating = Differences(value, pattern, ignoreCase);

            var diff = rating.Diff;

            if (rating.Diff == -1 || !ignoreCase) return rating.Diff;


            foreach (var ratingMatch in rating.Matches.Reverse())
            {
                value = value.Remove(ratingMatch.Index, ratingMatch.Length);
            }

            pattern = pattern.Replace("*", "").Replace("+", "").Replace("?", "");
            for (var i = 0; i < pattern.Length; i++)
            {
                if (pattern[i] != value[i]) diff++;
            }

            return diff;
        }

        
    }
}
