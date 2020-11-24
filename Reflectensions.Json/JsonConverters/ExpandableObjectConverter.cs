using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.ComTypes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Reflectensions.ExtensionMethods;
using Reflectensions.HelperClasses;

namespace Reflectensions.JsonConverters {
    public class ExpandableObjectConverter : JsonConverter {

        public override bool CanConvert(Type objectType) {

            return objectType == typeof(ExpandableObject) || objectType.InheritFromClass(typeof(ExpandableObject), true);
        }

        public override bool CanWrite { get; } = false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {

            var exoObj = (IDictionary<string, object>)Activator.CreateInstance(objectType);

            if (reader.TokenType == JsonToken.StartObject) {
                var jobject = JObject.Load(reader);
                //var dict = Json.Converter.ToDictionary(jobject);


                foreach (var pair in jobject) {

                    exoObj[pair.Key] = pair.Value?.ToExpandableObjectProperty();


                }
                //foreach (var keyValuePair in dict) {

                //    exoObj[keyValuePair.Key] = keyValuePair.Value;
                //}
            }


            return exoObj;
        }




    }
}
