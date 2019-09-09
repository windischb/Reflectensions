using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Reflectensions.HelperClasses {

    public class StringDifferences {

        public int Diff => _failed ? -1 : _diff;
        public IEnumerable<(int Index, int Length, string Value)> Matches => _failed ? null : _matches;


        private readonly List<(int Index, int Length, string Value)> _matches = new List<(int Index, int Length, string Value)>();
        private int _diff;
        private bool _failed;

        public StringDifferences(string value, string pattern, RegexOptions options) {

            var matches = new Regex(Wildcard.WildcardToRegex(pattern), options).Matches(value ?? "");

            if (matches == null || matches.Count == 0) {
                _failed = true;
                return;
            };

            foreach (Match match in matches) {
                for (int i = 1; i < match.Groups.Count; i++) {
                    AddMatch(match.Groups[i].Index, match.Groups[i].Length, match.Groups[i].Value);
                }
            }

        }

        public StringDifferences(string value, string pattern, bool ignoreCase) : this(value, pattern, ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None) {
            
        }

        public StringDifferences(string value, string pattern) : this(value, pattern, true) {

        }


        private StringDifferences AddMatch(int index, int length, string value) {
            _matches.Add((index, length, value));
            _diff += length != 0 ? length : 1;
            return this;
        }

    }

}
