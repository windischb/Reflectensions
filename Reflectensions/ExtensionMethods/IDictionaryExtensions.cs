using System;
using System.Collections.Generic;

namespace Reflectensions.ExtensionMethods {
    public static class IDictionaryHelpers {

        public static T GetValueAs<T>(IDictionary<string, object> dictionary, string key, T orDefault = default) {
            if(TryGetValueAs<T>(dictionary, key, out var val))
                return val;

            return orDefault;
        }

        public static bool TryGetValueAs<T>(IDictionary<string, object> dictionary, string key, out T value) {

            if (!dictionary.ContainsKey(key)) {
                value = default;
                return false;
            }

            if (ObjectHelpers.TryConvertTo<T>(dictionary[key], out var converted)) {
                value = converted;
                return true;
            }

            value = default;
            return false;
        }


        public static Dictionary<string, T> ToCaseInsensitiveDictionary<T>(IDictionary<string, T> dict) {

            return new Dictionary<string, T>(dict, StringComparer.OrdinalIgnoreCase);
        }

       
    }
}
