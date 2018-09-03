using System;
using Reflectensions.Helpers;

namespace Reflectensions.ExtensionMethods {
    internal static class JsonExtensions {

        private static MethodManager _methodManager = new MethodManager(options => options.EnableCache());

        private static Type _jsonType = null;


        static JsonExtensions() {
            _jsonType = TypeHelper.FindType("Reflectensions.Json");
        }

        public static bool IsAvailable => _jsonType != null;

        public static object ConvertTo(object @object, Type type) {

            var jsonInstance = _jsonType.CreateInstance();

            var json = _methodManager.InvokeMethod<string>(jsonInstance, "ToJson", @object);
            return _methodManager.InvokeMethod<object>(jsonInstance, "ToObject", json, type);

        }

        public static T ConvertTo<T>(object @object) {

            var jsonInstance = _jsonType.CreateInstance();

            var json = _methodManager.InvokeMethod<string>(jsonInstance, "ToJson", @object);
            return (T)_methodManager.InvokeMethod<object>(jsonInstance, "ToObject", json, typeof(T));

        }

        public static T ConvertTo<T>(string json) {

            var jsonInstance = _jsonType.CreateInstance();

            return (T)_methodManager.InvokeGenericMethod<T, T>(jsonInstance, "ToObject", json);

        }

    }


}
