using System.Reflection;
using System.Runtime.Serialization;

namespace Reflectensions
{

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

            if (methodType == MethodType.Instance)
                return BindingFlags.Instance;

            if (methodType == MethodType.Static)
                return BindingFlags.Static;

            return flags;

        }
    }
}
