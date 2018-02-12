using System;

namespace doob.Reflectensions {

    internal static class Throw {

        public static void IfEmpty(string value, string name) {
            if (String.IsNullOrWhiteSpace(value)) throw new ArgumentNullException(name);
        }

        public static bool IfIsNull(object value, string name, bool throwOnError = true) {
            if (value == null) {
                if (throwOnError) {
                    throw new ArgumentNullException(name);
                }

                return true;
            }

            return false;
        }
        
    }

}
