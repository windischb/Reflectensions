using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Reflectensions.Helper {
    public static class BindingFlagsHelper {
        public static BindingFlags Add(BindingFlags bindingFlags, BindingFlags additionalBindingFlags) {
            return bindingFlags | additionalBindingFlags;
        }
    }
}
