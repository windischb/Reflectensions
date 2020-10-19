using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Reflectensions.ExtensionMethods;
using Reflectensions.HelperClasses;

namespace Reflectensions.JsonConverters
{
    public class ExpandableObjectConverter : JsonConverter {
        private string BaseType { get; } = "Expandable.ExpandableObject";
        public override bool CanConvert(Type objectType)
        {
           
            return objectType == typeof(ExpandableObject) || objectType.InheritFromClass(typeof(ExpandableObject));
        }

        public override bool CanWrite { get; } = false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {

            
            var jobject = JObject.Load(reader);
            var dict = Json.Converter.ToDictionary(jobject);
            var exoObj = (ExpandableObject)Activator.CreateInstance(objectType);
            foreach (var keyValuePair in dict) {
                exoObj[keyValuePair.Key] = keyValuePair.Value;
            }

            return exoObj;
        }

        private Type GetBaseType() {
            return TypeExtensions.FindType(BaseType);
        }
    }
}
