using System;
using System.Reflection;

namespace Reflectensions.ExtensionMethods {
    public static class TypePropertyExtensions {

        public static T GetPropertyValue<T>(this object @object, string fieldName, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance) {
            var type = @object.GetType();

            var property = GetProperty(type, fieldName, bindingFlags);
            var value = property.GetValue(@object);
            return value.To<T>();
        }



        public static void SetPropertyValue(this object @object, string fieldName, object value, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance) {
            var type = @object.GetType();

            var property = GetProperty(type, fieldName, bindingFlags);
            property.SetValue(@object, value);
        }

        public static PropertyInfo GetProperty(Type t, string name, BindingFlags flags) {

            var field = t.GetProperty(name, flags);
            if (field != null)
                return field;

            if (t.BaseType != null)
                return GetProperty(t.BaseType, name, flags);

            return null;

        }

    }
}
