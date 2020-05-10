using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Reflectensions.ExtensionMethods {
    public static class StreamExtensions {

        private static T AsJsonToObject<T>(this Stream stream)
        {
            var json = new Json();
            return json.FromJsonStreamToObject<T>(stream);
        }

    }
}
