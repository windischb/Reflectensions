using System.Reflection;

namespace doob.Reflectensions.HelperClasses
{
    public class RatedMethodInfo
    {
        public MethodSearch Search { get; internal set; }
        public MethodInfo MethodInfo { get; internal set; }
        public MethodInfoRating Rating { get; internal set; }

        internal RatedMethodInfo(MethodInfo methodInfo, MethodSearch search) {
            Search = search;
            MethodInfo = methodInfo;
            Rating = MethodSignature.FromMethodInfo(methodInfo).RateAgainst(search, search.Context.SearchFor);
        }
    }
}
