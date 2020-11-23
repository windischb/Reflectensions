using System;
using System.Linq;
using Reflectensions.ExtensionMethods;

namespace Reflectensions.Helper {
    public static class JsonHelpers {

        private static IJson _json = null;

        static JsonHelpers() {
            Initialize();
        }

        private static bool initialized = false;
        static void Initialize() {

            if (initialized) {
                return;
            }
            initialized = true;

            var _jsonType = TypeHelper.FindType("Reflectensions.Json");

            if (_jsonType != null) {
                _json = (IJson)Activator.CreateInstance(_jsonType, new object[] {true});
            }

        }

        public static bool IsAvailable() {
            Initialize();
            return _json != null;
        }

        public static IJson Json() {
            Initialize();

            return _json;
        }

    }


}
