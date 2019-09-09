using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Reflectensions.ExtensionMethods;
using Reflectensions.HelperClasses;

namespace Reflectensions.Helpers {
    public static class TypeHelpers {
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

        #region Check
        public static bool IsNumericType(Type type, bool throwOnError = true) {
            if (Throw.IfIsNull(type, nameof(type), throwOnError))
                return false;

            return NumericTypes.Contains(type);
        }

        public static bool IsNumericType<T>(bool throwOnError = true) {
            return IsNumericType(typeof(T), throwOnError);
        }

        public static bool IsGenericTypeOf(Type type, Type genericType, bool throwOnError = true) {


            if (Throw.IfIsNull(type, nameof(type), throwOnError))
                return false;

            return type.IsGenericType && type.GetGenericTypeDefinition() == genericType;
        }

        public static bool IsGenericTypeOf<T>(Type type, bool throwOnError = true) {
            return IsGenericTypeOf(type, typeof(T), throwOnError);
        }
        

        public static bool IsNullableType(Type type, bool throwOnError = true) {
            if (Throw.IfIsNull(type, nameof(type), throwOnError))
                return false;

            return IsGenericTypeOf(type, typeof(Nullable<>));
        }


        public static bool IsNullableType<T>(bool throwOnError = true) {
            return IsNullableType(typeof(T), throwOnError);
        }

        public static bool IsEnumerableType(Type type, bool throwOnError = true) {

            if (Throw.IfIsNull(type, nameof(type), throwOnError))
                return false;

            if (type == typeof(string))
                return false;

            if (IsDictionaryType(type))
                return false;

            return type.GetInterfaces().Contains(typeof(IEnumerable));
        }

        public static bool IsEnumerableType<T>(bool throwOnError = true) {
            return IsEnumerableType(typeof(T), throwOnError);
        }



        public static bool IsDictionaryType(Type type, bool throwOnError = true) {
            if (Throw.IfIsNull(type, nameof(type), throwOnError))
                return false;


            return IsGenericTypeOf(type, typeof(IDictionary)) || IsGenericTypeOf(type, typeof(IDictionary<,>)) || ImplementsInterface(type, typeof(IDictionary)) || ImplementsInterface(type, typeof(IDictionary<,>));
        }

        public static bool IsDictionaryType<T>(bool throwOnError = true) {
            return IsDictionaryType(typeof(T), throwOnError);
        }

        public static bool ImplementsInterface<T>(Type type, bool throwOnError = true) {
            return ImplementsInterface(type, typeof(T), throwOnError);
        }
        public static bool ImplementsInterface(Type type, Type interfaceType, bool throwOnError = true) {
            if (Throw.IfIsNull(type, nameof(type), throwOnError) || Throw.IfIsNull(interfaceType, nameof(interfaceType), throwOnError))
                return false;

            if (!interfaceType.IsInterface) {
                if (throwOnError) {
                    throw new ArgumentException($"Paramter '{nameof(interfaceType)}' must be an Interface Type!");
                }
                return false;
            }

            return type.GetInterfaces().Select(i => i.IsGenericType ? i.GetGenericTypeDefinition() : i).Contains(interfaceType);
        }





        public static bool InheritFromClass<T>(Type type, bool inherit = false, bool throwOnError = true) {
            return InheritFromClass(type, typeof(T), inherit, throwOnError);
        }
        public static bool InheritFromClass(Type type, Type from, bool inherit = false, bool throwOnError = true) {

            int? maximumlevel = null;
            if (!inherit)
                maximumlevel = 0;

            return InheritFromClassLevel(type, from, maximumlevel, throwOnError) > 0;

        }


        public static bool IsImplicitCastableTo<T>(Type type, bool throwOnError = true) {
            return IsImplicitCastableTo(type, typeof(T), throwOnError);
        }
        public static bool IsImplicitCastableTo(this Type type, Type to, bool throwOnError = true) {

            if (Throw.IfIsNull(type, nameof(type), throwOnError) || Throw.IfIsNull(to, nameof(to), throwOnError))
                return false;

            var castable = false;

            if (IsNumericType(type) && IsNumericType(to)) {
                castable = ImplicitNumericConversionsTable[type].Contains(to);
            }

            if (!castable) {
                castable = GetImplicitCastMethodTo(type, to) != null;
            }

            return castable;

        }

        public static bool Equals<T>(Type type) {
            return type == typeof(T);
        }
        public static bool NotEquals<T>(Type type) {
            return !Equals<T>(typeof(T));
        }

        public static bool HasAttribute<T>(Type type, bool inherit = false, bool throwOnError = true) where T : Attribute {
            if (Throw.IfIsNull(type, nameof(type), throwOnError))
                return false;

            return HasAttribute(type, typeof(T));
        }
        public static bool HasAttribute(Type type, Type attributeType, bool inherit = false, bool throwOnError = true) {
            if (Throw.IfIsNull(type, nameof(type), throwOnError))
                return false;

            if (!InheritFromClass<Attribute>(attributeType, false)) {
                throw new ArgumentException($"Parameter '{nameof(attributeType)}' has to be an Attribute Type!");
            }

            return type.GetCustomAttribute(attributeType, inherit) != null;
        }


        public static bool IsStatic(Type type) {
            if (type == null)
                return false;
            return type.IsAbstract && type.IsSealed;
        }

        #endregion


        public static Type GetUnderlyingType(Type type) {

            if (!IsNullableType(type, false))
                throw new ArgumentException($"'{type}' is not a Nullable Type...");


            return Nullable.GetUnderlyingType(type);
        }

        public static IEnumerable<Type> HasAttribute<T>(IEnumerable<Type> types, bool inherit = false) where T : Attribute {
            return types.Where(m => HasAttribute<T>(m, inherit, false));
        }
        public static IEnumerable<Type> HasAttribute(IEnumerable<Type> types, Type attributeType, bool inherit = false) {
            return types.Where(m => HasAttribute(m, attributeType, inherit, false));
        }

        public static IEnumerable<(Type type, T Attribute)> WithAttribute<T>(IEnumerable<Type> types, bool inherit = false) where T : Attribute {
            return HasAttribute<T>(types, inherit).Select(t => (t, t.GetCustomAttribute<T>()));
        }
        public static IEnumerable<(Type Type, Attribute Attribute)> WithAttribute(IEnumerable<Type> types, Type attributeType, bool inherit = false) {

            if (!InheritFromClass<Attribute>(attributeType, true, false)) {
                throw new ArgumentException($"Parameter '{nameof(attributeType)}' be an Attribute Type!");
            }

            return HasAttribute(types, attributeType, inherit).Select(t => (t, t.GetCustomAttribute(attributeType)));
        }

        public static IEnumerable<Type> InheritFromClass(IEnumerable<Type> types, Type from, bool inherit = false) {
            return types.Where(t => InheritFromClass(t, from, inherit, false));
        }
        public static IEnumerable<Type> InheritFromClass<T>(IEnumerable<Type> types, bool inherit = false) {
            return types.Where(t => InheritFromClass<T>(t, inherit, false));
        }

        public static IEnumerable<Type> IsGenericTypeOf(IEnumerable<Type> types, Type of, bool throwOnError = false) {
            return types.Where(t => IsGenericTypeOf(t, of, throwOnError));
        }
        public static IEnumerable<Type> IsGenericTypeOf<T>(IEnumerable<Type> types, bool throwOnError = false) {
            return types.Where(t => IsGenericTypeOf<T>(t, throwOnError));
        }

        #region Query


        public static IEnumerable<MethodInfo> GetImplictOperatorMethods(Type type, bool throwOnError = true) {
            if (Throw.IfIsNull(type, nameof(type), throwOnError))
                return new List<MethodInfo>();

            return type.GetMethods(BindingFlags.Public | BindingFlags.Static).Where(m => MethodInfoHelpers.HasName(m, "op_Implicit"));
        }
        public static IEnumerable<MethodInfo> GetExplicitOperatorMethods(Type type, bool throwOnError = true) {
            if (Throw.IfIsNull(type, nameof(type), throwOnError))
                return new List<MethodInfo>();

            return type.GetMethods(BindingFlags.Public | BindingFlags.Static).Where(m => MethodInfoHelpers.HasName(m, "op_Explicit"));
        }

        public static MethodInfo GetImplicitCastMethodTo<T>(Type fromType, bool throwOnError = true) {
            return GetImplicitCastMethodTo(fromType, typeof(T), throwOnError);
        }
        public static MethodInfo GetImplicitCastMethodTo(Type type, Type to, bool throwOnError = true) {
            if (Throw.IfIsNull(type, nameof(type), throwOnError) || Throw.IfIsNull(to, nameof(to), throwOnError))
                return null;

            return GetImplictOperatorMethods(type, throwOnError).HasReturnType(to, throwOnError).HasParametersOfType(type).FirstOrDefault() ??
                     GetImplictOperatorMethods(to, throwOnError).HasReturnType(to, throwOnError).HasParametersOfType(type).FirstOrDefault();

        }

        public static MethodInfo GetExplicitCastMethodTo<T>(Type fromType, bool throwOnError = true) {
            return GetExplicitCastMethodTo(fromType, typeof(T), throwOnError);
        }
        public static MethodInfo GetExplicitCastMethodTo(Type type, Type to, bool throwOnError = true) {
            if (Throw.IfIsNull(type, nameof(type), throwOnError) || Throw.IfIsNull(to, nameof(to), throwOnError))
                return null;

            return GetExplicitOperatorMethods(type, throwOnError).HasReturnType(to, throwOnError).HasParametersOfType(type).FirstOrDefault() ??
                     GetExplicitOperatorMethods(to, throwOnError).HasReturnType(to, throwOnError).HasParametersOfType(type).FirstOrDefault();
        }
        #endregion

        public static int InheritFromClassLevel<T>(Type type, int? maximumLevel = null, bool throwOnError = true) {
            return InheritFromClassLevel(type, typeof(T), maximumLevel, throwOnError);
        }
        public static int InheritFromClassLevel(Type type, Type from, int? maximumLevel = null, bool throwOnError = true) {
            if (Throw.IfIsNull(type, nameof(type), throwOnError) || Throw.IfIsNull(type, nameof(from), throwOnError))
                return -1;

            lock (TypeHelperCache.InheritanceListLock) {
                var exists = TypeHelperCache.InheritanceList.FirstOrDefault(item => item?.Type == type && item?.From == from);
                if (exists != null)
                    return exists.Value.Level;

                var level = 0;
                bool found = type == from;
                var loockupType = type;

                if (loockupType.IsGenericType) {
                    found = loockupType.GetGenericTypeDefinition() == from;
                }

                while (loockupType != null && !found && (!maximumLevel.HasValue || level >= maximumLevel.Value)) {
                    level++;

                    found = loockupType.BaseType == from;
                    loockupType = loockupType.BaseType;
                    if (loockupType?.IsGenericType == true) {
                        found = loockupType.GetGenericTypeDefinition() == from;
                    }

                }

                if (!found)
                    level = -1;

                TypeHelperCache.InheritanceList.Add((type, from, level));
                return level;
            }

        }

        public static Type[] ToParameterTypes(IEnumerable<object> objects) {
            return objects?.Select(o => o?.GetType() ?? typeof(NullObject)).ToArray();
        }


        public static object CreateInstance(Type type, params object[] args) {
            if (type == null)
                return null;

            return Activator.CreateInstance(type, args);
        }


        public static IEnumerable<MethodInfo> GetImplictOperatorMethods<T>() {
            return GetImplictOperatorMethods(typeof(T));
        }

        public static IEnumerable<MethodInfo> GetExplicitOperatorMethods<T>() {
            return GetExplicitOperatorMethods(typeof(T));
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
