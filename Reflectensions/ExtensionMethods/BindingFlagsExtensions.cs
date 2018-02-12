using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace doob.Reflectensions.ExtensionMethods
{
    public static class BindingFlagsExtensions
    {
        public static BindingFlags Add(this BindingFlags bindingFlags, BindingFlags additionalBindingFlags) {
            return bindingFlags | additionalBindingFlags;
        }
    }
}
