using System;

namespace Reflectensions.Ratings
{
    public abstract class BaseRating<T> : IRating where T : BaseRating<T> {

        public bool Failed { get; protected set; }
        public abstract string Rating { get; }

        internal T SetFailed() {
            Failed = true;
            return (T)this;
        }


        public int CompareTo(IRating other) {
            var thisPieces = ToString().Split('.');
            var otherPieces = other.ToString().Split('.');

            if (thisPieces.Length != otherPieces.Length)
                throw new Exception($"Can't compare Rating: '{this}' with '{other}'!");

            for (var i = 0; i < thisPieces.Length; i++) {
                var thisNumber = int.Parse(thisPieces[i]);
                var otherNumber = int.Parse(otherPieces[i]);

                var comp = thisNumber.CompareTo(otherNumber);
                if (comp != 0) {
                    return comp;
                }

            }

            return 0;
        }

    }
}
