using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace doob.Reflectensions
{
    public static class ObjectExtensions
    {

        public static T CastTo<T>(this object @object, bool throwOnError = true, T returnOnError = default(T) ) {
            return (T)CastTo(@object, typeof(T), throwOnError, returnOnError);
        }

        public static object CastTo(this object @object, Type outType, bool throwOnError = true, object returnOnError = null) {
            if (@object == null)
                return null;

            var t = @object.GetType();

            try {

                if (@object.GetType().ImplementsInterface<IConvertible>() && outType.ImplementsInterface<IConvertible>()) {
                    return Convert.ChangeType(@object, outType);
                }

                if (t == outType) {
                    return @object;
                }

                if (t.ImplementsInterface(outType, false) || t.InheritFromClass(outType, true))
                    return @object;



                var method = t.GetImplicitCastMethodTo(outType);

                if (method != null) {
                    return method.Invoke(null, new object[] {
                        @object
                    });
                }

                if (outType.IsNullableType()) {
                    var underlingType = Nullable.GetUnderlyingType(outType);
                    try {
                        var casted = @object.CastTo(underlingType);
                        //var nullable = Activator.CreateInstance(outType, casted);
                        //var nt = nullable.GetType().IsNullableType();
                        ////nullable.GetType().GetProperty("Value").SetValue(nullable, casted);
                        //var valueExpr = Expression.Constant(casted, outType);
                        //var nt1 = valueExpr.Value.GetType().IsNullableType();
                        return casted;
                    } catch (Exception e) {
                        return Activator.CreateInstance(outType);
                    }

                }

                

            } catch (Exception e) {

                if (!throwOnError) {
                    if (returnOnError != null) {
                        return returnOnError;
                    }
                    return Activator.CreateInstance(outType);
                }
                    

                
            }

            throw new InvalidCastException($"Can't cast object of Type '{t}' to '{outType}'.");

        }

        public static bool TryCastToBoolean(this object value, params object[] trueValues) {

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


        public static bool EqualsToAny(this object value, params object[] equalsto) {

            foreach (var trueValue in equalsto) {
                if (value == trueValue) {
                    return true;
                }
            }

            return false;
        }
    }
}
