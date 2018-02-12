using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using doob.Reflectensions.Exceptions;
using doob.Reflectensions.HelperClasses;

namespace doob.Reflectensions.ExtensionMethods
{
    public static class RatedMethodInfoExtensions
    {
        public static MethodInfo FindBestMatchingMethodInfo(this IEnumerable<RatedMethodInfo> methodInfos, MethodSearch search, bool throwOnError = true) {

            var possibleMethods = methodInfos
                .GroupBy(p => p.Rating.ToString())
                .OrderBy(result => result.Key).ToList();

            if (!possibleMethods.Any()) {
                if (throwOnError) {
                    throw new MethodNotFoundException(search.Context.MethodName);
                }

                return null;
            }

            if (possibleMethods.First().Count() > 1) {
                if (throwOnError) {
                    throw new MultipleMethodsFoundException(search.Context.MethodName);
                }

                return null;
            }

            return possibleMethods.First().First().MethodInfo;
        }

    }
}
