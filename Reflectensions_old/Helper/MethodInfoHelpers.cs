using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Reflectensions.HelperClasses;

namespace Reflectensions.Helper {
    public static class MethodInfoHelpers {

        public static bool HasName(MethodInfo methodInfo, string name, bool throwOnError = true) {
            if (Throw.IfIsNull(methodInfo, nameof(methodInfo), throwOnError))
                return false;

            return methodInfo.Name == name;
        }

        public static int MatchesName(MethodInfo methodInfo, string name, bool throwOnError = true) {
            if (Throw.IfIsNull(methodInfo, nameof(methodInfo), throwOnError))
                return -1;

            Throw.IfEmpty(name, nameof(name));

            var searchValue = name.Contains(".") ? $"{methodInfo.ReflectedType}.{methodInfo.Name}" : methodInfo.Name;

            return StringHelpers.DifferencesCount(searchValue, name);
        }

        public static bool HasParametersLengthOf(MethodInfo methodInfo, int paramterLength, bool includeOptional = false, bool throwOnError = true) {
            if (Throw.IfIsNull(methodInfo, nameof(methodInfo), throwOnError))
                return false;

            var methodInfoParameters = methodInfo.GetParameters();

            if (methodInfoParameters.Length < paramterLength) {
                return false;
            }

            if (methodInfoParameters.Length == paramterLength) {
                return true;
            }
            
            return includeOptional && methodInfoParameters.Skip(paramterLength).All(p => p.IsOptional);
        }

        public static bool HasParametersOfType(MethodInfo methodInfo, Type[] types, bool throwOnError = true) {
            if (Throw.IfIsNull(methodInfo, nameof(methodInfo), throwOnError))
                return false;

            if (types == null) types = new Type[0];

            if (!HasParametersLengthOf(methodInfo, types.Length, true)) return false;

            var match = true;
            var methodParameters = methodInfo.GetParameters();
            for (var i = 0; i < methodParameters.Length; i++) {
                if (methodParameters[i].ParameterType != types[i]) {
                    match = false;
                    break;
                }
            }

            return match;
        }

        public static bool HasAttribute<T>(MethodInfo methodInfo, bool inherit = false, bool throwOnError = true) where T : Attribute {
            if (Throw.IfIsNull(methodInfo, nameof(methodInfo), throwOnError))
                return false;

            return HasAttribute(methodInfo, typeof(T));
        }

        public static bool HasAttribute(MethodInfo methodInfo, Type attributeType, bool inherit = false, bool throwOnError = true) {
            if (Throw.IfIsNull(methodInfo, nameof(methodInfo), throwOnError))
                return false;

            if (!TypeHelpers.InheritFromClass<Attribute>(attributeType, false)) {
                throw new ArgumentException($"Parameter '{nameof(attributeType)}' has to be an Attribute Type!");
            }

            return methodInfo.GetCustomAttribute(attributeType, inherit) != null;
        }

        public static bool HasReturnType<T>(MethodInfo methodInfo, bool throwOnError = true) {
            Throw.IfIsNull(methodInfo, nameof(methodInfo));
            return TypeHelpers.Equals<T>(methodInfo.ReturnType);
        }
        public static bool HasReturnType(MethodInfo methodInfo, Type returnType, bool throwOnError = true) {
            Throw.IfIsNull(methodInfo, nameof(methodInfo));
            Throw.IfIsNull(returnType, nameof(returnType));

            return methodInfo.ReturnType == returnType;
        }

        public static MethodAccessModifier GetAccessModifier(MethodInfo methodInfo) {
            var checkTypeValues = new List<MethodAttributes>() {
                MethodAttributes.Public,
                MethodAttributes.FamORAssem,
                MethodAttributes.Family,
                MethodAttributes.Assembly,
                MethodAttributes.FamANDAssem,
                MethodAttributes.Private

            };

            foreach (var value in checkTypeValues) {
                if ((methodInfo.Attributes & value) == value) {
                    switch (value) {
                        case MethodAttributes.Public: {
                            return MethodAccessModifier.Public;
                        }
                        case MethodAttributes.FamORAssem: {
                            return MethodAccessModifier.ProtectedInternal;
                        }
                        case MethodAttributes.Family: {
                            return MethodAccessModifier.Protected;
                        }
                        case MethodAttributes.Assembly: {
                            return MethodAccessModifier.Internal;
                        }
                        case MethodAttributes.FamANDAssem: {
                            return MethodAccessModifier.PrivateProtected;
                        }
                        case MethodAttributes.Private: {
                            return MethodAccessModifier.Private;
                        }
                        default: {
                            return MethodAccessModifier.Unknown;
                        }
                    }
                }
            }

            return MethodAccessModifier.Unknown;
        }

        //public static IEnumerable<RatedMethodInfo> FindMatchingMethodInfo(this IEnumerable<MethodInfo> methodInfos, MethodSearch search, bool throwOnError = true) {

        //    var possibleMethods = methodInfos
        //        .Select(m => new RatedMethodInfo(m, search))
        //        .Where(result => !result.Rating.Failed);

        //    return possibleMethods;

        //}

        //public static MethodInfo FindBestMatchingMethodInfo(this IEnumerable<MethodInfo> methodInfos, MethodSearch search, bool throwOnError = true) {

        //    return methodInfos.FindMatchingMethodInfo(search, throwOnError).FindBestMatchingMethodInfo(search, throwOnError);
        //}


        //public static IEnumerable<MethodInfo> HasName(this IEnumerable<MethodInfo> methodInfos, string name) {
        //    Throw.IfIsNull(methodInfos, nameof(methodInfos));

        //    return methodInfos.Where(m => m.HasName(name));
        //}

        public static IEnumerable<MethodInfo> HasReturnType<T>(this IEnumerable<MethodInfo> methodInfos, Type returnType, bool throwOnError = true) {
            if (Throw.IfIsNull(methodInfos, nameof(methodInfos), throwOnError))
                return null;

            if (returnType == null)
                returnType = typeof(void);

            return methodInfos.Where(m => HasReturnType<T>(m));
        }
        public static IEnumerable<MethodInfo> HasReturnType(this IEnumerable<MethodInfo> methodInfos, Type returnType, bool throwOnError = true) {
            if (Throw.IfIsNull(methodInfos, nameof(methodInfos), throwOnError))
                return null;

            if (returnType == null)
                returnType = typeof(void);

            return methodInfos.Where(m => HasReturnType(m, returnType));
        }

        public static IEnumerable<MethodInfo> HasParametersOfType(this IEnumerable<MethodInfo> methodInfos, params Type[] types) {
            return methodInfos.Where(m => HasParametersOfType(m, types));
        }

        public static IEnumerable<MethodInfo> HasAttribute<T>(this IEnumerable<MethodInfo> methodInfos, bool inherit = false) where T : Attribute {
            return methodInfos.Where(m => HasAttribute<T>(m, inherit));
        }
        public static IEnumerable<MethodInfo> HasAttribute(this IEnumerable<MethodInfo> methodInfos, Type attributeType, bool inherit = false) {
            return methodInfos.Where(m => HasAttribute(m, attributeType, inherit));
        }

        public static IEnumerable<(MethodInfo MethodInfo, T Attribute)> WithAttribute<T>(this IEnumerable<MethodInfo> methodInfos, bool inherit = false) where T : Attribute {
            return methodInfos.HasAttribute<T>().Select(t => (t, t.GetCustomAttribute<T>()));
        }
        public static IEnumerable<(MethodInfo MethodInfo, Attribute Attribute)> WithAttribute(this IEnumerable<MethodInfo> methodInfos, Type attributeType, bool inherit = false) {

            if (!TypeHelpers.InheritFromClass<Attribute>(attributeType, true, false)) {
                throw new ArgumentException($"Parameter '{nameof(attributeType)}' be an Attribute Type!");
            }

            return methodInfos.HasAttribute(attributeType).Select(t => (t, t.GetCustomAttribute(attributeType)));
        }


    }
}
