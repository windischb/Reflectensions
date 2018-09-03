using System;
using System.Collections.Generic;
using System.Reflection;
using Reflectensions.ExtensionMethods;

namespace Reflectensions.Helpers {
    public static class TypeHelper {
        #region Constants

        public static Dictionary<Type, List<Type>> ImplicitNumericConversionsTable = new Dictionary<Type, List<Type>> {
            { typeof(sbyte),   new List<Type> { typeof(short), typeof(int), typeof(long), typeof(float), typeof(double), typeof(decimal) } },
            { typeof(byte),    new List<Type> { typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal) } },
            { typeof(short),   new List<Type> { typeof(int), typeof(long), typeof(float), typeof(double), typeof(decimal) } },
            { typeof(ushort),  new List<Type> { typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal) } },
            { typeof(int),     new List<Type> { typeof(long), typeof(float), typeof(double), typeof(decimal) } },
            { typeof(uint),    new List<Type> { typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal) } },
            { typeof(long),    new List<Type> { typeof(float), typeof(double), typeof(decimal) } },
            { typeof(char),    new List<Type> { typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal) } },
            { typeof(float),   new List<Type> { typeof(double) } },
            { typeof(ulong),   new List<Type> { typeof(float), typeof(double), typeof(decimal) } },
            { typeof(double),  new List<Type>() },
            { typeof(decimal), new List<Type>() }
        };
        public static IEnumerable<Type> NumericTypes => ImplicitNumericConversionsTable.Keys;

        #endregion


        public static bool IsNumericType<T>() {
            return typeof(T).IsNumericType();
        }

        public static bool IsNullableType<T>() {
            return typeof(T).IsNullableType();
        }

        public static bool IsEnumerableType<T>() {
            return typeof(T).IsEnumerableType();
        }

        public static bool IsDictionaryType<T>() {
            return typeof(T).IsDictionaryType();
        }

        public static IEnumerable<MethodInfo> GetImplictOperatorMethods<T>() {
            return typeof(T).GetImplictOperatorMethods();
        }

        public static IEnumerable<MethodInfo> GetExplicitOperatorMethods<T>() {
            return typeof(T).GetExplicitOperatorMethods();
        }

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



    }
}
