using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace doob.Reflectensions.ExtensionMethods
{
    internal static class EnumExtensions
    {

        internal static string GetName(this Enum enumValue) {

            return enumValue.GetType()
                            .GetTypeInfo()
                            .DeclaredMembers
                            .SingleOrDefault(x => x.Name == enumValue.ToString())
                            .GetCustomAttribute<EnumMemberAttribute>(false)?.Value ?? enumValue.ToString();
        }

    }

}
