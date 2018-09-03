using System.Collections.Generic;
using System.Linq;

namespace Reflectensions.ExtensionMethods
{
    public static class ListExtensions
    {
        public static List<T> ToNull<T>(this List<T> list) {

            return list.Any() ? list : null;
        }
    }
}
