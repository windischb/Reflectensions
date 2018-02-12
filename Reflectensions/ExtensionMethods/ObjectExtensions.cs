using System;
using System.Collections.Generic;
using System.Text;

namespace doob.Reflectensions
{
    public static class ObjectExtensions
    {
        public static T CastTo<T>(this object @object)
        {
            return (T)CastTo(@object, typeof(T));
        }
        public static object CastTo(this object @object, Type outType)
        {

            var t = @object.GetType();

            if ( @object.GetType().ImplementsInterface<IConvertible>() && outType.ImplementsInterface<IConvertible>())
            {
                return Convert.ChangeType(@object, outType);
            }

            if (t == outType)
            {
                return @object;
            }

            if (t.ImplementsInterface(outType, false) || t.InheritFromClass(outType, true))
                return @object;



            var method = t.GetImplicitCastMethodTo(outType);

            if (method != null)
            {
                return method.Invoke(null, new object[] {
                    @object
                });
            }

            throw new InvalidCastException($"Can't cast object of Type '{t}' to '{outType}'.");
        }

    }
}
