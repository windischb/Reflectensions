using System;
using System.Collections.Generic;

namespace Reflectensions.ExtensionMethods {
    public static class IDictionaryExtensions {

        public static T GetValueAs<T>(this IDictionary<string, object> dictionary, string key, T orDefault = default) {
            if(dictionary.TryGetValueAs<T>(key, out var val))
                return val;

            return orDefault;
        }

        public static bool TryGetValueAs<T>(this IDictionary<string, object> dictionary, string key, out T value) {

            if (!dictionary.ContainsKey(key)) {
                value = default;
                return false;
            }

            if (dictionary[key].TryConvertTo<T>(out var converted)) {
                value = converted;
                return true;
            }

            value = default;
            return false;
        }


        public static IDictionary<string, T> ToCaseInsensitiveIDictionary<T>(this IDictionary<string, T> dict) {

            return new Dictionary<string, T>(dict, StringComparer.OrdinalIgnoreCase);
        }

        public static Dictionary<string, T> ToCaseInsensitiveDictionary<T>(this IDictionary<string, T> dict) {

            return new Dictionary<string, T>(dict, StringComparer.OrdinalIgnoreCase);
        }

       
    }
}
