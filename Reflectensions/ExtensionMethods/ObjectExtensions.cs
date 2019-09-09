using System;
using Reflectensions.Helpers;

namespace Reflectensions.ExtensionMethods {
    public static class ObjectExtensions {

        public static T ConvertTo<T>(this object @object, bool throwOnError = true, T returnOnError = default(T)) => ObjectHelpers.ConvertTo<T>(@object, throwOnError, returnOnError);

        public static object ConvertTo(this object @object, Type outType, bool throwOnError = true, object returnOnError = null) => ObjectHelpers.ConvertTo(@object, outType, throwOnError, returnOnError);

        public static bool TryConvertTo<T>(this object @object, out T outValue) => ObjectHelpers.TryConvertTo<T>(@object, out outValue);

        public static bool TryConvertTo(this object @object, Type outType, out object outValue) => ObjectHelpers.TryConvertTo(@object, outType, out outValue);

        public static bool TryConvertToBoolean(object value, params object[] trueValues) => ObjectHelpers.TryConvertToBoolean(value, trueValues);

        public static bool EqualsToAny(object value, params object[] equalsto) => ObjectHelpers.EqualsToAny(value, equalsto);
    }
}
