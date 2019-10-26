using System;
using Reflectensions.Helper;

namespace Reflectensions.ExtensionMethods {
    public static class ObjectExtensions {

        //public static T ConvertTo<T>(this object @object, bool throwOnError = true, T returnOnError = default(T)) => ObjectHelpers.ConvertTo<T>(@object, throwOnError, returnOnError);

        //public static object ConvertTo(this object @object, Type outType, bool throwOnError = true, object returnOnError = null) => ObjectHelpers.ConvertTo(@object, outType, throwOnError, returnOnError);

        //public static bool TryConvertTo<T>(this object @object, out T outValue) => ObjectHelpers.TryConvertTo<T>(@object, out outValue);

        //public static bool TryConvertTo(this object @object, Type outType, out object outValue) => ObjectHelpers.TryConvertTo(@object, outType, out outValue);

        public static bool TryConvertToBoolean(object value, params object[] trueValues) => ObjectHelpers.TryConvertToBoolean(value, trueValues);

        public static bool EqualsToAnyOf(object value, params object[] equalsto) => ObjectHelpers.EqualsToAnyOf(value, equalsto);


        public static bool TryAs(this object @object, Type outType, out object outValue) => ObjectHelpers.TryAs(@object, outType, out outValue);
        public static bool TryAs<T>(this object @object, out T outValue) => ObjectHelpers.TryAs<T>(@object, out outValue);
        public static object As(this object @object, Type outType) => ObjectHelpers.As(@object, outType);
        public static T As<T>(this object @object) => ObjectHelpers.As<T>(@object);





        public static bool TryTo(this object @object, Type outType, out object outValue) => ObjectHelpers.TryTo(@object, outType, out outValue);
        public static bool TryTo<T>(this object @object, out T outValue) => ObjectHelpers.TryTo<T>(@object, out outValue);
        public static object To(this object @object, Type outType, bool throwOnError = true, object returnOnError = null) => ObjectHelpers.To(@object, outType, throwOnError, returnOnError);
        public static T To<T>(this object @object, bool throwOnError = true, T returnOnError = default(T)) => ObjectHelpers.To<T>(@object, throwOnError, returnOnError);


    }
}
