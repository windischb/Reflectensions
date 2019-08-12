using System.Reflection;
using Reflectensions.Helper;

namespace Reflectensions.ExtensionMethods
{
    public static class BindingFlagsExtensions
    {
        public static BindingFlags Add(this BindingFlags bindingFlags, BindingFlags additionalBindingFlags) => BindingFlagsHelper.Add(bindingFlags, additionalBindingFlags);
    }

    
}
