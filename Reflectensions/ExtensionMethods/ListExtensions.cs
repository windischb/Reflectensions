using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace doob.Reflectensions
{
    public static class ListExtensions
    {
        public static List<T> ToNull<T>(this List<T> list) {

            return list.Any() ? list : null;
        }
    }
}
