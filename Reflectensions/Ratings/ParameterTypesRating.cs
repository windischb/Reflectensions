namespace Reflectensions.Ratings
{
    public class ParameterTypesRating : BaseRating<ParameterTypesRating> {

        public int ExactTypesMismatches => _parameterCount - _exactMatch;
        public int InheritTypesMismatches => CastableTypesMismatch - _inheritanceCount;
        public int InheritanceLevel => _inheritanceLevel;
        public int CastableTypesMismatch => ExactTypesMismatches - _castableCount;
        public int GenericParameterCount => _genericParameterCount;

        private int _parameterCount;
        private int _exactMatch;
        private int _inheritanceCount;
        private int _inheritanceLevel;
        private int _castableCount;
        private int _genericParameterCount;

        internal ParameterTypesRating SetGenericParameterCount(int value) {
            _genericParameterCount = value;
            return this;
        }


        internal ParameterTypesRating AddExactMatch() {
            _parameterCount++;
            _exactMatch++;
            return this;
        }

        internal ParameterTypesRating AddInheritanceLevel(int level) {
            _parameterCount++;

            if (level < 0) {
                return SetFailed();
            }

            _inheritanceCount++;
            _inheritanceLevel = _inheritanceLevel + level;
            return this;
        }

        internal ParameterTypesRating AddCastableMatch() {
            _parameterCount++;
            _castableCount++;
            return this;
        }

        public override string ToString() {
            return Rating;
        }

        public override string Rating =>
            $"{ExactTypesMismatches}.{InheritTypesMismatches}.{CastableTypesMismatch}.{InheritanceLevel}.{GenericParameterCount}".Trim(new char[]{' ','.'});
    }
}
