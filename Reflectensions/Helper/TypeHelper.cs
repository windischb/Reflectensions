using System;
using System.Collections.Generic;
using System.Text;
using Reflectensions.ExtensionMethods;
using Reflectensions.HelperClasses;

namespace Reflectensions.Helper {
    public static class TypeHelper {

        public static Type FindType(string typeName) {

            if (string.IsNullOrWhiteSpace(typeName))
                return typeof(void);


            lock (TypeHelperCache.TypeFromStringLock) {
                if (TypeHelperCache.TypeFromString.ContainsKey(typeName))
                    return TypeHelperCache.TypeFromString[typeName];

                Type foundType = null;

                if (!typeName.Contains(".")) {
                    foundType = Type.GetType($"System.{typeName}", false, true);
                }

                if (foundType == null) {
                    var assemblies = AppDomain.CurrentDomain.GetAssemblies();

                    foreach (var assembly in assemblies) {

                        foundType = assembly.GetType(typeName, false, false);
                        if (foundType != null) {
                            break;
                        }

                    }

                    if (foundType == null) {
                        foreach (var assembly in assemblies) {
                            foundType = assembly.GetType(typeName, false, true);
                            if (foundType != null) {
                                break;
                            }
                        }
                    }
                }

                TypeHelperCache.TypeFromString.Add(typeName, foundType);

                return foundType;
            }

        }

        public static Type GetUnderlyingType<T>() {

            return typeof(T).GetEnumUnderlyingType();
        }

        public static bool IsNumericType<T>(bool throwOnError = true) {
            return typeof(T).IsNumericType(throwOnError);
        }

        public static bool IsNullableType<T>(bool throwOnError = true) {
            return typeof(T).IsNullableType(throwOnError);
        }

        public static bool IsEnumerableType<T>(bool throwOnError = true) {
            return typeof(T).IsEnumerableType(throwOnError);
        }

        public static bool IsDictionaryType<T>(bool throwOnError = true) {
            return typeof(T).IsDictionaryType(throwOnError);
        }
    }
}
