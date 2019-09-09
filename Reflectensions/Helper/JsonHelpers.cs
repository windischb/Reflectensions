using System;

namespace Reflectensions.Helpers {
    public static class JsonHelpers {

        private static Type _jsonType = null;
        private static Func<object, string> ToJsonMethod = null;
        private static Func<string, Type, object> ToObjectMethod = null;

        static JsonHelpers() {
            Initialize();
        }

        private static bool initialized = false;
        static void Initialize() {

            if (initialized) {
                return;
            }
            initialized = true;

            _jsonType = TypeHelpers.FindType("Reflectensions.Json");

            if(!IsAvailable) {
                return;
            }

            var jsonInstance = TypeHelpers.CreateInstance(_jsonType);

            var toJsonMethod = jsonInstance.GetType().GetMethod("ToJson");
            ToJsonMethod = (Func<object, string>) Delegate.CreateDelegate(typeof(Func<object, string>), "ToJson", toJsonMethod);

            var toObjectMethod = jsonInstance.GetType().GetMethod("ToObject");
            ToObjectMethod = (Func<string, Type, object>)Delegate.CreateDelegate(typeof(Func<string, Type, object>), "ToObject", toObjectMethod);

        }

        public static bool IsAvailable => _jsonType != null;

        public static object ConvertTo(object @object, Type type) {

            Initialize();

            var json = ToJsonMethod(@object);
            return ToObjectMethod(json, type);

        }

        public static T ConvertTo<T>(object @object) {

            Initialize();

            return (T)ConvertTo(@object, typeof(T));
        }

        public static T ConvertTo<T>(string json) {

            Initialize();

            return (T)ToObjectMethod(json, typeof(T));

        }

    }


}
