using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace doob.Reflectensions.HelperClasses {

    internal class Enum<T> where T : struct, IConvertible
    {

        public static T Find(string value) {
            var fields = typeof(T).GetFields();

            foreach (var field in fields)
            {
                var attribute = field.GetCustomAttribute<EnumMemberAttribute>();
                if (attribute != null)
                {
                    if (attribute.Value == value)
                        return (T)field.GetValue(null);
                }
                else if (field.Name == value)
                {
                    return (T)field.GetValue(null);
                }
            }

            return default(T);
        }
        
    }

}
