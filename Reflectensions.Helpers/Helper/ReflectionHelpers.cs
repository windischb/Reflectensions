using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Reflectensions.Helpers {
    public static class ReflectionHelpers {

        public static T GetPropertyValue<T>(object @object, string fieldName, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance) {
            var type = @object.GetType();

            var property = GetProperty(type, fieldName, bindingFlags);
            var value = property.GetValue(@object);
            return (T)value;
        }

        public static T GetPropertyValue<T>(Type type, string fieldName, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance) {
            var property = type.GetProperty(fieldName, bindingFlags);
            var value = property.GetValue(type);
            return (T)value;
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
