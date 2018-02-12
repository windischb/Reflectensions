﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace doob.Reflectensions
{
    internal static class TypeHelperCache
    {
        
        internal static readonly Dictionary<string, Type> TypeFromString = new Dictionary<string, Type>();
        internal static readonly object TypeFromStringLock = new object();

        internal static readonly List<(Type Type, Type From, int Level)?> InheritanceList = new List<(Type Type, Type From, int Level)?>();
        internal static readonly object InheritanceListLock = new object();
        
    }
}
