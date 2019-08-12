using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Reflectensions.HelperClasses;
using Reflectensions.Helpers;

namespace Reflectensions.ExtensionMethods {
    public static class EnumExtensions {

        public static string GetName(this Enum enumValue) => EnumHelpers.GetName(enumValue);
        public static bool TryFind(this Type enumType, string value, out object result) => EnumHelpers.TryFind(enumType, value, out result);
        public static bool TryFind(Type enumType, string value, bool ignoreCase, out object result) => EnumHelpers.TryFind(enumType, value, ignoreCase, out result);

    }

}
