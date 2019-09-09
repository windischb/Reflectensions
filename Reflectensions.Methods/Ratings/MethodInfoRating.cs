namespace Reflectensions.Ratings {
    public class MethodInfoRating : BaseRating<MethodInfoRating> {

        public MethodNameRating MethodNameRating { get; private set; }
        public ParameterTypesRating ParameterTypesMismatch { get; private set; }


        internal MethodInfoRating SetMethodNameRating(MethodNameRating rating) {
            MethodNameRating = rating;
            if (rating.Failed)
                Failed = true;

            return this;
        }
        internal MethodInfoRating SetParameterTypesRating(ParameterTypesRating rating) {

            ParameterTypesMismatch = rating;
            if (rating.Failed)
                Failed = true;

            return this;
        }
        
        public override string ToString() {
            return Rating;
        }

        public override string Rating => $"{MethodNameRating}.{ParameterTypesMismatch}".Trim(new char[] { ' ', '.' });
    }
}
