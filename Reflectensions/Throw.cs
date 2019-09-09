using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Reflectensions {

    public static class Throw {

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


        public static void Ignore(Action operation) {
            if (operation == null)
                return;

            try {
                operation.Invoke();
            } catch { }

        }

        public static T Ignore<T>(Func<T> operation) {
            return Ignore<T>(operation, default);
        }

        public static T Ignore<T>(Func<T> operation, T defaultValue) {
            
            if (operation == null)
                return defaultValue;

            try {
                return operation.Invoke();
            } catch {
                return defaultValue;
            }

        }

    }

}
