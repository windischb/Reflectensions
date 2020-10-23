using System;
using System.Collections.Generic;
using System.Reflection;
using Reflectensions.HelperClasses;

namespace Reflectensions.ExtensionMethods {
    public static class TypePropertyExtensions {

        //public static T GetPropertyValue<T>(this object @object, string fieldName, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance) {
        //    var type = @object.GetType();

        //    var property = GetProperty(type, fieldName, bindingFlags);
        //    var value = property.GetValue(@object);
        //    return value.To<T>();
        //}
        public static object GetPropertyValue(this object @object, string name, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance) {
            return GetPropertyValue<object>(@object, name, bindingFlags);
        }
        public static T GetPropertyValue<T>(this object @object, string name, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance) {

            var parts = name.Split('.');

            var currentObject = @object;
            var processedPaths = new List<string>();
            foreach (var part in parts) {
                processedPaths.Add(part);
                var currentPropertyInfo = currentObject.GetType().GetProperty(part, bindingFlags);


                if (currentPropertyInfo == null)
                    throw new Exception($"Path not found '{string.Join(".", processedPaths)}'");

                currentObject = currentPropertyInfo.GetValue(currentObject);
            }

            return currentObject.To<T>();

        }

        public static void SetPropertyValue(this object @object, string path, object value, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance) {
            var parts = path.Split('.');

            var currentObject = @object;
            var processedPaths = new List<string>();

            for (var i = 0; i < parts.Length; i++) {
                var part = parts[i];
                var isLast = i == parts.Length-1;

                processedPaths.Add(part);
                var currentPropertyInfo = currentObject.GetType().GetProperty(part, bindingFlags);


                if (currentPropertyInfo == null)
                    throw new Exception($"Path not found '{string.Join(".", processedPaths)}'");

                if (isLast) {
                    currentPropertyInfo.SetValue(currentObject, value);
                    return;
                } else {
                    currentObject = currentPropertyInfo.GetValue(currentObject);
                }
                
            }

        }

        //public static PropertyInfo GetProperty(Type t, string name, BindingFlags flags) {

        //    var parts = name.Split('.');

        //    Type currentType = t;
        //    PropertyInfo currentPropertyInfo = null;
        //    var processedPaths = new List<string>();
        //    foreach (var part in parts) {
        //        processedPaths.Add(part);
        //        currentPropertyInfo = currentType.GetProperty(part, flags);
                
        //        if(currentPropertyInfo == null)
        //            throw new Exception($"Path not found '{String.Join(".", processedPaths)}'");

        //        currentType = currentPropertyInfo.PropertyType;
        //    }

        //    return currentPropertyInfo;

        //    //var field = t.GetProperty(name, flags);
        //    //if (field != null)
        //    //    return field;

        //    //if (t.BaseType != null)
        //    //    return GetProperty(t.BaseType, name, flags);

        //    //return null;

        //}


        //public static T GetPropertyValue<T>(this object @object, string path, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance) {
        //    var resolver = new PropertyResolver();
        //    var result = resolver.Resolve(@object, path, bindingFlags);
        //    return result.To<T>();
        //}
    }
}
