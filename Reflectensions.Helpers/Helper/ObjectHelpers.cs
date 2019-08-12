using System;
using Reflectensions.Helpers;

namespace Reflectensions.ExtensionMethods {
    public static class ObjectHelpers {

        public static T ConvertTo<T>(object @object, bool throwOnError = true, T returnOnError = default(T)) {
            return (T)ConvertTo(@object, typeof(T), throwOnError, returnOnError);
            
        }

        public static object ConvertTo(object @object, Type outType, bool throwOnError = true, object returnOnError = null) {

           
            var result = TryConvertTo(@object, outType, out var outValue);
            if (result)
                return outValue;


            if (!throwOnError) {
                if (returnOnError != null) {
                    return returnOnError;
                }
                return outType.IsValueType ? Activator.CreateInstance(outType) : null;
            }
            throw new InvalidCastException($"Can't cast object of Type '{@object.GetType()}' to '{outType}'.");

        }

        public static bool TryConvertTo<T>(object @object, out T outValue) {


            var result = TryConvertTo(@object, typeof(T), out var _outValue);

            outValue = _outValue != null ? (T)_outValue : default(T);
            return result;
        }

        public static bool TryConvertTo(object @object, Type outType, out object outValue) {


            if (@object == null) {
                outValue = null;
                return false;
            }


            var t = @object.GetType();

            if (t == outType) {
                outValue = @object;
                return true;
            }

            if (TypeHelpers.ImplementsInterface(t, outType, false) || TypeHelpers.InheritFromClass(t, outType, true)) {
                outValue = @object;
                return true;
            }


            try {



                if (TypeHelpers.ImplementsInterface<IConvertible>(@object?.GetType()) && TypeHelpers.ImplementsInterface<IConvertible>(outType)) {
                    outValue = Convert.ChangeType(@object, outType);
                    return true;
                }

                var method = TypeHelpers.GetImplicitCastMethodTo(t, outType);

                if (method != null) {
                    outValue = method.Invoke(null, new object[] {
                        @object
                    });
                    return true;
                }



                if (TypeHelpers.IsNullableType(outType)) {
                    var underlingType = Nullable.GetUnderlyingType(outType);
                    if (TryConvertTo(@object, underlingType, out var innerValue)) {
                        outValue = innerValue;
                        return true;
                    } else {
                        if (!JsonHelpers.IsAvailable) {
                            outValue = Activator.CreateInstance(outType);
                            return false;
                        }
                        
                    }
                    
                }

                if (JsonHelpers.IsAvailable) {
                    try {
                        outValue = JsonHelpers.ConvertTo(@object, outType);
                        return true;
                    } catch {
                        outValue = null;
                        return false;
                    }

                }



            } catch {

                outValue = null;
                return false;

            }

            outValue = null;
            return false;


        }

      
        public static bool TryConvertToBoolean(object value, params object[] trueValues) {

            if (EqualsToAny(trueValues))
                return true;

            if (value == null)
                return false;

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


        public static bool EqualsToAny(object value, params object[] equalsto) {

            foreach (var trueValue in equalsto) {
                if (value == trueValue) {
                    return true;
                }
            }

            return false;
        }
    }
}
