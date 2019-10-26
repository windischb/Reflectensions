using System;
using System.Linq;

namespace Reflectensions.ExtensionMethods {
    public static class ObjectExtensions {


        public static bool ToBoolean(this object value, params object[] trueValues) {

            if (value == null)
                return false;

            if (trueValues.Any()) {
                return EqualsToAnyOf(trueValues);
            }
            

            if (value is bool boolValue)
                return boolValue;



            var str = value.ToString();
            if (int.TryParse(str, out var numb)) {
                return numb > 0;
            }

            if (bool.TryParse(str, out var ret)) {
                return ret;
            }


            if (str.ToLower() == "yes") {
                return true;
            }

            return ret;
        }


        public static bool EqualsToAnyOf(this object value, params object[] equalsto) {

            foreach (var trueValue in equalsto) {
                if (value == trueValue) {
                    return true;
                }
            }

            return false;
        }


        public static bool IsCastableTo(this object value, Type type) {
            return value.GetType().IsImplicitCastableTo(type, false);
        }
        public static bool IsCastableTo<T>(this object value) {
            return value.GetType().IsImplicitCastableTo<T>(false);
        }


        public static bool TryAs(this object value, Type type, out object outValue) {

            if (value == null) {
                outValue = null;
                return false;
            }

            var t = value.GetType();

            if (t == type) {
                outValue = value;
                return true;
            }

            if (t.ImplementsInterface(type, false) || t.InheritFromClass(type, true)) {
                outValue = value;
                return true;
            }

            var method = t.GetImplicitCastMethodTo(type, false);

            if (method != null) {
                outValue = method.Invoke(null, new object[] {
                    value
                });
                return true;
            }

            if (type.IsNullableType()) {
                var underlingType = Nullable.GetUnderlyingType(type);
                if (TryAs(value, underlingType, out var innerValue)) {
                    outValue = innerValue;
                    return true;
                }
            }

            outValue = null;
            return false;

        }

        public static bool TryAs<T>(this object value, out T outValue) {

            var result = TryAs(value, typeof(T), out var _outValue);

            outValue = _outValue != null ? (T)_outValue : default;
            return result;
        }

        public static object As(this object value, Type type) {

            var result = TryAs(value, type, out var outValue);
            return result ? outValue : null;

        }

        public static T As<T>(this object value) {

            var result = TryAs<T>(value, out var outValue);
            return result ? outValue : default;

        }



        public static bool TryTo(this object value, Type type, out object outValue) {

            if (type == typeof(bool)) {
                outValue = ToBoolean(value);
                return true;
            }
            
            if (TryAs(value, type, out var _outValue)) {
                outValue = _outValue;
                return true;
            }


            if (type.IsNullableType(false)) {
                type = type.GetUnderlyingType();
            }

            if (value.GetType().ImplementsInterface<IConvertible>() && type.ImplementsInterface<IConvertible>()) {
                try {
                    outValue = Convert.ChangeType(value, type);
                    return true;
                } catch {
                    // ignored
                }
            }

            var method = value.GetType().GetImplicitCastMethodTo(type, false);

            if (method != null) {
                outValue = method.Invoke(null, new object[] {
                    value
                });
                return true;
            }

            outValue = null;
            return false;

        }

        public static bool TryTo<T>(this object value, out T outValue) {

            var result = TryTo(value, typeof(T), out var _outValue);

            outValue = _outValue != null ? (T)_outValue : default;
            return result;
        }

        public static object To(this object value, Type type, bool throwOnError = true, object returnOnError = null) {

            var result = TryTo(value, type, out var outValue);
            if (result)
                return outValue;


            if (!throwOnError) {
                if (returnOnError != null) {
                    return returnOnError;
                }
                return type.IsValueType ? Activator.CreateInstance(type) : null;
            }
            throw new InvalidCastException($"Can't cast object of Type '{value.GetType()}' to '{type}'.");

        }

        public static T To<T>(this object value, bool throwOnError = true, T returnOnError = default) {
            return (T)To(value, typeof(T), throwOnError, returnOnError);
        }


    }
}
