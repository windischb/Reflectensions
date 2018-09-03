using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace Reflectensions
{
    [Flags]
    public enum MethodAccessModifier
    {
        [EnumMember(Value = "unknown")]
        Unknown = 0,

        [EnumMember(Value = "public")]
        Public = 1,

        [EnumMember(Value = "protected")]
        Protected = 2,

        [EnumMember(Value = "internal")]
        Internal = 4,

        [EnumMember(Value = "protected internal")]
        ProtectedInternal = 8,

        [EnumMember(Value = "private")]
        Private = 16,

        [EnumMember(Value = "private protected")]
        PrivateProtected = 32,

        [EnumMember(Value = "*")]
        Any = Public | Protected | Internal | ProtectedInternal | Private | PrivateProtected

        
    }

    public static class MethodAccessModifierExtensions {

        public static BindingFlags ToBindingFlags(this MethodAccessModifier accessModifier) {

            BindingFlags flags = BindingFlags.Default;

            if (accessModifier.HasFlag(MethodAccessModifier.Public))
                flags = flags | BindingFlags.Public;

            if (accessModifier.HasFlag(MethodAccessModifier.Protected))
                flags = flags | BindingFlags.NonPublic;

            if (accessModifier.HasFlag(MethodAccessModifier.Internal))
                flags = flags | BindingFlags.NonPublic;

            if (accessModifier.HasFlag(MethodAccessModifier.ProtectedInternal))
                flags = flags | BindingFlags.NonPublic;

            if (accessModifier.HasFlag(MethodAccessModifier.Private))
                flags = flags | BindingFlags.NonPublic;

            if (accessModifier.HasFlag(MethodAccessModifier.PrivateProtected))
                flags = flags | BindingFlags.NonPublic;

            return flags;

        }
    }
}
