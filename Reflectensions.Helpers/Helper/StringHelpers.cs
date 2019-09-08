using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Reflectensions.Helpers {
    public static class StringHelpers {

        public static string[] Split(string value, string split, StringSplitOptions options) {

            if (value == null) {
                return new string[0];
            }

            return value.Split(new[] { split }, options);

        }

        public static string[] Split(string value, string split, bool removeEmptyEntries = false) {
            return Split(value, split, removeEmptyEntries ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None);
        }


        public static StringDifferences Differences(string value, string pattern, bool ignoreCase = true) {

            return new StringDifferences(value, pattern, ignoreCase);

        }

        public static int DifferencesCount(string value, string pattern) {

            if (string.IsNullOrWhiteSpace(value))
                return -1;

            var ignoreCase = pattern.StartsWith("@");
            pattern = pattern.TrimStart('@');
            var rating = Differences(value, pattern, ignoreCase);

            var diff = rating.Diff;

            if (rating.Diff == -1 || !ignoreCase) return rating.Diff;


            foreach (var ratingMatch in rating.Matches.Reverse()) {
                value = value.Remove(ratingMatch.Index, ratingMatch.Length);
            }

            pattern = pattern.Replace("*", "").Replace("+", "").Replace("?", "");
            for (var i = 0; i < pattern.Length; i++) {
                if (pattern[i] != value[i]) diff++;
            }

            return diff;
        }


    }
}
