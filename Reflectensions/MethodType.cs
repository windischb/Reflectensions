using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace doob.Reflectensions
{
    [Flags]
    public enum MethodType
    {

        [EnumMember(Value = "")]
        Instance,

        [EnumMember(Value = "static")]
        Static
    }

    public static class MethodTypeExtensions {

        public static BindingFlags ToBindingFlags(this MethodType methodType) {

            BindingFlags flags = BindingFlags.Default;

            if (methodType.HasFlag(MethodType.Instance))
                flags = flags | BindingFlags.Instance;

            if (methodType.HasFlag(MethodType.Static))
                flags = flags | BindingFlags.Static;

            return flags;

        }
    }
}
