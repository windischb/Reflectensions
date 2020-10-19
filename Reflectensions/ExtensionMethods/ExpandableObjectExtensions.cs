using System.Collections.Generic;
using System.Linq;
using Reflectensions.HelperClasses;

namespace Reflectensions.ExtensionMethods {
    public static class ExpandableObjectExtensions {

        public static Dictionary<string, object> AsDictionary(this ExpandableObject expandableObject) {
            return expandableObject.GetProperties().ToDictionary(kv => kv.Key, kv => kv.Value);
        }

    }
}
