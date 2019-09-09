using System;
using System.Collections.Generic;
using System.Reflection;
using Reflectensions.Helper;

namespace Reflectensions.ExtensionMethods {
    public static class TypeExtensions {

        #region Check
        public static bool IsNumericType(this Type type, bool throwOnError = true) => TypeHelpers.IsNumericType(type, throwOnError);
        public static bool IsGenericTypeOf(this Type type, Type genericType, bool throwOnError = true) => TypeHelpers.IsGenericTypeOf(type, genericType, throwOnError);
        public static bool IsGenericTypeOf<T>(this Type type, bool throwOnError = true) => TypeHelpers.IsGenericTypeOf<T>(type, throwOnError);
        public static bool IsNullableType(this Type type, bool throwOnError = true) => TypeHelpers.IsNullableType(type, throwOnError);
        public static bool IsEnumerableType(this Type type, bool throwOnError = true) => TypeHelpers.IsEnumerableType(type, throwOnError);
        public static bool IsDictionaryType(this Type type, bool throwOnError = true) => TypeHelpers.IsDictionaryType(type, throwOnError);
        public static bool ImplementsInterface<T>(this Type type, bool throwOnError = true) => TypeHelpers.ImplementsInterface<T>(type, throwOnError);
        public static bool ImplementsInterface(this Type type, Type interfaceType, bool throwOnError = true) => TypeHelpers.ImplementsInterface(type, interfaceType, throwOnError);
        public static bool InheritFromClass<T>(this Type type, bool inherit = false, bool throwOnError = true) => TypeHelpers.InheritFromClass<T>(type, inherit, throwOnError);
        public static bool InheritFromClass(this Type type, Type from, bool inherit = false, bool throwOnError = true) => TypeHelpers.InheritFromClass(type, from, inherit, throwOnError);
        public static bool IsImplicitCastableTo<T>(this Type type, bool throwOnError = true) => TypeHelpers.IsImplicitCastableTo<T>(type, throwOnError);
        public static bool IsImplicitCastableTo(this Type type, Type to, bool throwOnError = true) => TypeHelpers.IsImplicitCastableTo(type, to, throwOnError);
        public static bool Equals<T>(this Type type) => TypeHelpers.Equals<T>(type);
        public static bool NotEquals<T>(this Type type) => TypeHelpers.NotEquals<T>(type);
        public static bool HasAttribute<T>(this Type type, bool inherit = false, bool throwOnError = true) where T : Attribute => TypeHelpers.HasAttribute<T>(type, inherit, throwOnError);
        public static bool HasAttribute(this Type type, Type attributeType, bool inherit = false, bool throwOnError = true) => TypeHelpers.HasAttribute(type, attributeType, inherit, throwOnError);
        public static bool IsStatic(this Type type) => TypeHelpers.IsStatic(type);

        #endregion


        public static Type GetUnderlyingType(this Type type) => TypeHelpers.GetUnderlyingType(type);




        public static IEnumerable<Type> HasAttribute<T>(this IEnumerable<Type> types, bool inherit = false) where T : Attribute => TypeHelpers.HasAttribute<T>(types, inherit);
        public static IEnumerable<Type> HasAttribute(this IEnumerable<Type> types, Type attributeType, bool inherit = false) => TypeHelpers.HasAttribute(types, attributeType, inherit);
        public static IEnumerable<(Type type, T Attribute)> WithAttribute<T>(this IEnumerable<Type> types, bool inherit = false) where T : Attribute => TypeHelpers.WithAttribute<T>(types, inherit);
        public static IEnumerable<(Type Type, Attribute Attribute)> WithAttribute(this IEnumerable<Type> types, Type attributeType, bool inherit = false) => TypeHelpers.WithAttribute(types, attributeType, inherit);
        public static IEnumerable<Type> InheritFromClass(this IEnumerable<Type> types, Type from, bool inherit = false) => TypeHelpers.InheritFromClass(types, from, inherit);
        public static IEnumerable<Type> InheritFromClass<T>(this IEnumerable<Type> types, bool inherit = false) => TypeHelpers.InheritFromClass<T>(types, inherit);
        public static IEnumerable<Type> IsGenericTypeOf(this IEnumerable<Type> types, Type of, bool throwOnError = false) => TypeHelpers.IsGenericTypeOf(types, of, throwOnError);
        public static IEnumerable<Type> IsGenericTypeOf<T>(this IEnumerable<Type> types, bool throwOnError = false) => TypeHelpers.IsGenericTypeOf<T>(types, throwOnError);

        #region Query


        public static IEnumerable<MethodInfo> GetImplictOperatorMethods(this Type type, bool throwOnError = true) => TypeHelpers.GetImplictOperatorMethods(type, throwOnError);
        public static IEnumerable<MethodInfo> GetExplicitOperatorMethods(this Type type, bool throwOnError = true) => TypeHelpers.GetExplicitOperatorMethods(type, throwOnError);


        public static MethodInfo GetImplicitCastMethodTo<T>(this Type fromType, bool throwOnError = true) => TypeHelpers.GetImplicitCastMethodTo<T>(fromType, throwOnError);
        public static MethodInfo GetImplicitCastMethodTo(this Type type, Type to, bool throwOnError = true) => TypeHelpers.GetImplicitCastMethodTo(type, to, throwOnError);

        public static MethodInfo GetExplicitCastMethodTo<T>(this Type fromType, bool throwOnError = true) => TypeHelpers.GetExplicitCastMethodTo<T>(fromType, throwOnError);
        public static MethodInfo GetExplicitCastMethodTo(this Type type, Type to, bool throwOnError = true) => TypeHelpers.GetExplicitCastMethodTo(type, to, throwOnError);
        #endregion

        public static int InheritFromClassLevel<T>(this Type type, int? maximumLevel = null, bool throwOnError = true) => TypeHelpers.InheritFromClassLevel<T>(type, maximumLevel, throwOnError);

        public static int InheritFromClassLevel(this Type type, Type from, int? maximumLevel = null, bool throwOnError = true) => TypeHelpers.InheritFromClassLevel(type, from, maximumLevel, throwOnError);

        public static Type[] ToParameterTypes(this IEnumerable<object> objects) => TypeHelpers.ToParameterTypes(objects);


        public static object CreateInstance(this Type type, params object[] args) => TypeHelpers.CreateInstance(type, args);


    }
}
