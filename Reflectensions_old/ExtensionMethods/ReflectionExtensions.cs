using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Reflectensions.Helper;

namespace Reflectensions.ExtensionMethods {
    public static class ReflectionExtensions {

        public static T GetPropertyValue<T>(this object @object, string fieldName, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance) => ReflectionHelpers.GetPropertyValue<T>(@object, fieldName, bindingFlags);
        public static T GetPropertyValue<T>(this Type type, string fieldName, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance) => ReflectionHelpers.GetPropertyValue<T>(type, fieldName, bindingFlags);
        public static PropertyInfo GetProperty(Type t, string name, BindingFlags flags) => ReflectionHelpers.GetProperty(t, name, flags);

    }
}
