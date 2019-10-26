using System.Reflection;

namespace Reflectensions.ExtensionMethods {
    public static class BindingFlagsExtensions {
        public static BindingFlags Add(this BindingFlags bindingFlags, BindingFlags additionalBindingFlags) {
            return bindingFlags | additionalBindingFlags;
        }
    }
}
