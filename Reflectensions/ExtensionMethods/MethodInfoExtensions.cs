using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Reflectensions.HelperClasses;

namespace Reflectensions.ExtensionMethods {
    public static class MethodInfoExtensions {

        public static bool HasName(this MethodInfo methodInfo, string name, bool throwOnError = true) {
            if (methodInfo == null) {
                if (throwOnError) {
                    throw new ArgumentNullException(nameof(methodInfo));
                }
                return false;
            }

            return methodInfo.Name == name;
        }

        public static bool HasParametersLengthOf(this MethodInfo methodInfo, int paramterLength, bool includeOptional = false, bool throwOnError = true) {
            if (methodInfo == null) {
                if (throwOnError) {
                    throw new ArgumentNullException(nameof(methodInfo));
                }
                return false;
            }

            var methodInfoParameters = methodInfo.GetParameters();

            if (methodInfoParameters.Length < paramterLength) {
                return false;
            }

            if (methodInfoParameters.Length == paramterLength) {
                return true;
            }
            
            return includeOptional && methodInfoParameters.Skip(paramterLength).All(p => p.IsOptional);
        }

        public static bool HasParametersOfType(this MethodInfo methodInfo, Type[] types, bool throwOnError = true) {
            if (methodInfo == null) {
                if (throwOnError) {
                    throw new ArgumentNullException(nameof(methodInfo));
                }
                return false;
            }

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

        public static bool HasAttribute<T>(this MethodInfo methodInfo, bool inherit = false, bool throwOnError = true) where T : Attribute {
            return HasAttribute(methodInfo, typeof(T), inherit, throwOnError);
        }

        public static bool HasAttribute(this MethodInfo methodInfo, Type attributeType, bool inherit = false, bool throwOnError = true) {
            if (methodInfo == null) {
                if (throwOnError) {
                    throw new ArgumentNullException(nameof(methodInfo));
                }
                return false;
            }

            if (!attributeType.InheritFromClass<Attribute>(inherit, throwOnError)) {
                throw new ArgumentException($"Parameter '{nameof(attributeType)}' has to be an Attribute Type!");
            }

            return methodInfo.GetCustomAttribute(attributeType, inherit) != null;
        }

        public static bool HasReturnType<T>(this MethodInfo methodInfo, bool throwOnError = true) {
            if (methodInfo == null) {
                if (throwOnError) {
                    throw new ArgumentNullException(nameof(methodInfo));
                }
                return false;
            }
            return methodInfo.ReturnType.Equals<T>();
        }
        
        public static bool HasReturnType(this MethodInfo methodInfo, Type returnType, bool throwOnError = true) {
            if (methodInfo == null) {
                if (throwOnError) {
                    throw new ArgumentNullException(nameof(methodInfo));
                }
                return false;
            }

            if (returnType == null) {
                if (throwOnError) {
                    throw new ArgumentNullException(nameof(returnType));
                }
                return false;
            }

            return methodInfo.ReturnType == returnType;
        }

        public static MethodAccessModifier GetAccessModifier(this MethodInfo methodInfo) {
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

        public static IEnumerable<MethodInfo> WithName(this IEnumerable<MethodInfo> methodInfos, string name) {
            return methodInfos.Where(m => m.HasName(name, false));
        }

       

        public static IEnumerable<MethodInfo> WithReturnType<T>(this IEnumerable<MethodInfo> methodInfos) {
            return WithReturnType(methodInfos, typeof(T));
        }
        
        public static IEnumerable<MethodInfo> WithReturnType(this IEnumerable<MethodInfo> methodInfos, Type returnType) {
            return methodInfos.Where(m => HasReturnType(m, returnType, false));
        }

        public static IEnumerable<MethodInfo> WithParametersOfType(this IEnumerable<MethodInfo> methodInfos, params Type[] types) {
            return methodInfos.Where(m => HasParametersOfType(m, types));
        }

        public static IEnumerable<MethodInfo> WithAttribute<T>(this IEnumerable<MethodInfo> methodInfos, bool inherit = false) where T : Attribute {
            return methodInfos.Where(m => HasAttribute<T>(m, inherit));
        }
        
        public static IEnumerable<MethodInfo> WithAttribute(this IEnumerable<MethodInfo> methodInfos, Type attributeType, bool inherit = false) {
            return methodInfos.Where(m => HasAttribute(m, attributeType, inherit));
        }

        public static IEnumerable<(MethodInfo MethodInfo, T Attribute)> WithAttributeExpanded<T>(this IEnumerable<MethodInfo> methodInfos, bool inherit = false) where T : Attribute {
            return WithAttribute<T>(methodInfos, inherit).Select(t => (t, t.GetCustomAttribute<T>()));
        }
        
        public static IEnumerable<(MethodInfo MethodInfo, Attribute Attribute)> WithAttributeExpanded(this IEnumerable<MethodInfo> methodInfos, Type attributeType, bool inherit = false) {

            if (!attributeType.InheritFromClass<Attribute>(inherit, false)) {
                throw new ArgumentException($"Parameter '{nameof(attributeType)}' be an Attribute Type!");
            }

            return WithAttribute(methodInfos, attributeType, inherit).Select(t => (t, t.GetCustomAttribute(attributeType)));
        }


    }
}
