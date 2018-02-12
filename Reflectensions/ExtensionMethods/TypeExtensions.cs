using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using doob.Reflectensions.HelperClasses;
using doob.Reflectensions.Helpers;

namespace doob.Reflectensions {
    public static class TypeExtensions {

        

        #region Check
        
        public static bool IsNumericType(this Type type, bool throwOnError = true) {
            if (Throw.IfIsNull(type, nameof(type), throwOnError))
                return false;

            return TypeHelper.NumericTypes.Contains(type);
        }

        public static bool IsNullableType(this Type type, bool throwOnError = true) {
            if (Throw.IfIsNull(type, nameof(type), throwOnError))
                return false;

            return IsGenericTypeOf(type, typeof(Nullable<>));
        }

        public static bool IsGenericTypeOf<T>(this Type type, bool throwOnError = true) {
            return IsGenericTypeOf(type, typeof(T), throwOnError);
        }
        public static bool IsGenericTypeOf(this Type type, Type genericType, bool throwOnError = true) {


            if (Throw.IfIsNull(type, nameof(type), throwOnError))
                return false;

            return type.IsGenericType && type.GetGenericTypeDefinition() == genericType;
        }

        public static bool IsEnumerableType(this Type type, bool throwOnError = true) {

            if (Throw.IfIsNull(type, nameof(type), throwOnError))
                return false;

            if (IsDictionaryType(type))
                return false;

            return type.GetInterfaces().Contains(typeof(IEnumerable));
        }

        public static bool IsDictionaryType(this Type type, bool throwOnError = true) {
            if (Throw.IfIsNull(type, nameof(type), throwOnError))
                return false;


            return ImplementsInterface(type, typeof(IDictionary)) || ImplementsInterface(type, typeof(IDictionary<,>));
        }

        public static bool ImplementsInterface<T>(this Type type, bool throwOnError = true) {
            return ImplementsInterface(type, typeof(T), throwOnError);
        }
        public static bool ImplementsInterface(this Type type, Type interfaceType, bool throwOnError = true) {
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

        public static bool InheritFromClass<T>(this Type type, bool inherit = false, bool throwOnError = true) {
            return InheritFromClass(type, typeof(T), throwOnError);
        }
        public static bool InheritFromClass(this Type type, Type from, bool inherit = false, bool throwOnError = true) {

            int? maximumlevel = null;
            if (!inherit)
                maximumlevel = 0;

            return TypeExtensions.InheritFromClassLevel(type, from, maximumlevel, throwOnError) > -1;

        }

        public static bool IsImplicitCastableTo<T>(this Type type, bool throwOnError = true) {
            return IsImplicitCastableTo(type, typeof(T), throwOnError);
        }
        public static bool IsImplicitCastableTo(this Type type, Type to, bool throwOnError = true) {

            if (Throw.IfIsNull(type, nameof(type), throwOnError) || Throw.IfIsNull(to, nameof(to), throwOnError))
                return false;

            var castable = false;

            if (IsNumericType(type) && IsNumericType(to)) {
                castable = TypeHelper.ImplicitNumericConversionsTable[type].Contains(to);
            }

            if (!castable) {
                castable = TypeExtensions.GetImplicitCastMethodTo(type, to) != null;
            }

            return castable;

        }

        public static bool Equals<T>(this Type type) {
            return type.Equals(typeof(T));
        }
        public static bool NotEquals<T>(this Type type) {
            return !type.Equals<T>();
        }

        public static bool HasAttribute<T>(this Type type, bool inherit = false, bool throwOnError = true) where T : Attribute {
            if (Throw.IfIsNull(type, nameof(type), throwOnError))
                return false;

            return HasAttribute(type, typeof(T));
        }
        public static bool HasAttribute(this Type type, Type attributeType, bool inherit = false, bool throwOnError = true) {
            if (Throw.IfIsNull(type, nameof(type), throwOnError))
                return false;

            if (!InheritFromClass<Attribute>(attributeType, false)) {
                throw new ArgumentException($"Parameter '{nameof(attributeType)}' has to be an Attribute Type!");
            }

            return type.GetCustomAttribute(attributeType, inherit) != null;
        }

        #endregion


        #region Query

       
        public static IEnumerable<MethodInfo> GetImplictOperatorMethods(this Type type, bool throwOnError = true) {
            if (Throw.IfIsNull(type, nameof(type), throwOnError))
                return new List<MethodInfo>();

            return type.GetMethods(BindingFlags.Public | BindingFlags.Static).HasName("op_Implicit");
        }
        public static IEnumerable<MethodInfo> GetExplicitOperatorMethods(this Type type, bool throwOnError = true) {
            if (Throw.IfIsNull(type, nameof(type), throwOnError))
                return new List<MethodInfo>();

            return type.GetMethods(BindingFlags.Public | BindingFlags.Static).HasName("op_Explicit");
        }

        public static MethodInfo GetImplicitCastMethodTo<T>(this Type fromType, bool throwOnError = true) {
            return GetImplicitCastMethodTo(fromType, typeof(T), throwOnError);
        }
        public static MethodInfo GetImplicitCastMethodTo(this Type type, Type to, bool throwOnError = true) {
            if (Throw.IfIsNull(type, nameof(type), throwOnError) || Throw.IfIsNull(to, nameof(to), throwOnError))
                return null;

            return type.GetImplictOperatorMethods(throwOnError).HasReturnType(to, throwOnError).HasParametersOfType(type).FirstOrDefault() ??
                     to.GetImplictOperatorMethods(throwOnError).HasReturnType(to, throwOnError).HasParametersOfType(type).FirstOrDefault();

        }

        public static MethodInfo GetExplicitCastMethodTo<T>(this Type fromType, bool throwOnError = true) {
            return GetExplicitCastMethodTo(fromType, typeof(T), throwOnError);
        }
        public static MethodInfo GetExplicitCastMethodTo(this Type type, Type to, bool throwOnError = true) {
            if (Throw.IfIsNull(type, nameof(type), throwOnError) || Throw.IfIsNull(to, nameof(to), throwOnError))
                return null;

            return type.GetExplicitOperatorMethods(throwOnError).HasReturnType(to, throwOnError).HasParametersOfType(type).FirstOrDefault() ??
                     to.GetExplicitOperatorMethods(throwOnError).HasReturnType(to, throwOnError).HasParametersOfType(type).FirstOrDefault();
        }
        #endregion

        public static int InheritFromClassLevel<T>(this Type type, int? maximumLevel = null, bool throwOnError = true) {
            return InheritFromClassLevel(type, typeof(T), maximumLevel, throwOnError);
        }
        public static int InheritFromClassLevel(this Type type, Type from, int? maximumLevel = null, bool throwOnError = true) {
            if (Throw.IfIsNull(type, nameof(type), throwOnError) || Throw.IfIsNull(type, nameof(from), throwOnError))
                return -1;

            lock (TypeHelperCache.InheritanceListLock) {
                var exists = TypeHelperCache.InheritanceList.FirstOrDefault(item => item?.Type == type && item?.From == from);
                if (exists != null)
                    return exists.Value.Level;

                var level = 0;
                bool found = type == from;

                var loockupType = type;
                while (loockupType != null && !found && (!maximumLevel.HasValue || level >= maximumLevel.Value)) {
                    level++;
                    found = loockupType.BaseType == from;
                    loockupType = loockupType.BaseType;
                }

                if (!found)
                    level = -1;

                TypeHelperCache.InheritanceList.Add((type, from, level));
                return level;
            }

        }

        public static Type[] ToParameterTypes(this IEnumerable<object> objects) {
            return objects?.Select(o => o?.GetType() ?? typeof(NullObject)).ToArray();
        }

    }
}
