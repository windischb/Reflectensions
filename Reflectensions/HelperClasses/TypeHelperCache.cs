using System;
using System.Collections.Generic;

namespace Reflectensions.HelperClasses
{
    internal static class TypeHelperCache
    {
        
        internal static Dictionary<string, Type> TypeFromString { get; } = new Dictionary<string, Type>();
        internal static object TypeFromStringLock { get; } = new object();

        internal static List<(Type Type, Type From, int Level)?> InheritanceList { get; } = new List<(Type Type, Type From, int Level)?>();
        internal static object InheritanceListLock { get; } = new object();
        
    }
}
