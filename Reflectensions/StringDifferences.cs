using System.Collections.Generic;

namespace Reflectensions {

    public class StringDifferences {

        public int Diff => _failed ? -1 : _diff;
        public IEnumerable<(int Index, int Length, string Value)> Matches => _failed ? null : _matches;
        

        private readonly List<(int Index, int Length, string Value)> _matches = new List<(int Index, int Length, string Value)>();
        private int _diff;
        private bool _failed;

        internal StringDifferences SetFailed()
        {
            _failed = true;
            return this;
        }


        internal StringDifferences AddMatch(int index, int length, string value) {
            _matches.Add((index, length, value));
            _diff += length != 0 ? length : 1;
            return this;
        }

    }

}
