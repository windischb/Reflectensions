using System.Collections.Generic;
using Reflectensions.ExtensionMethods;

namespace Reflectensions.Helper {
    public static class IDictionaryExtensions {

        public static T GetValueAs<T>(this IDictionary<string, object> dictionary, string key, T orDefault = default) => IDictionaryHelpers.GetValueAs<T>(dictionary, key, orDefault);

        public static bool TryGetValueAs<T>(this IDictionary<string, object> dictionary, string key, out T value) => IDictionaryHelpers.TryGetValueAs<T>(dictionary, key, out value);


        public static Dictionary<string, T> ToCaseInsensitiveDictionary<T>(this IDictionary<string, T> dict) => IDictionaryHelpers.ToCaseInsensitiveDictionary<T>(dict);

    }
}
