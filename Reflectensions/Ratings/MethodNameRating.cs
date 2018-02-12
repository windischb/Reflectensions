namespace doob.Reflectensions
{
    public class MethodNameRating : BaseRating<MethodNameRating> {
        public int? MethodNameMismatches => _namingDiff;
        private int? _namingDiff;

        internal MethodNameRating SetNamingDiff(int diff) {

            if (diff < 0) {
                SetFailed();
                return this;
            }

            _namingDiff = diff;
            return this;
        }

        public override string ToString() {
            return Rating;
        }

        public override string Rating => $"{MethodNameMismatches}".Trim(new char[] { ' ', '.' });
    }
}
