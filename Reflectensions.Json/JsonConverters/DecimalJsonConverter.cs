using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Reflectensions.ExtensionMethods;

namespace Reflectensions.JsonConverters {
    class DecimalJsonConverter : JsonConverter {

        public override bool CanRead => true;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {

            var dec = reader.ReadAsString();
            if (dec.IsInt()) {
                return dec.ToInt();
            }

            if (dec.IsLong()) {
                return dec.ToLong();
            }

            if (dec.IsDouble()) {
                return dec.ToDouble();
            }

            if (decimal.TryParse(dec, out var deci)) {
                return deci;
            }

            return Convert.ChangeType(dec, objectType);
        }

        public override bool CanConvert(Type objectType) {
            return (objectType == typeof(decimal) || objectType == typeof(float) || objectType == typeof(double) || objectType == typeof(int) || objectType == typeof(long));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            if (DecimalJsonConverter.IsWholeValue(value)) {
                writer.WriteRawValue(JsonConvert.ToString(Convert.ToInt64(value)));
            } else {
                writer.WriteRawValue(JsonConvert.ToString(value));
            }
        }

        private static bool IsWholeValue(object value) {
            if (value is decimal) {
                decimal decimalValue = (decimal)value;
                int precision = (Decimal.GetBits(decimalValue)[3] >> 16) & 0x000000FF;
                return precision == 0;
            } else if (value is float || value is double) {
                double doubleValue = Convert.ToDouble(value);
                return doubleValue == Math.Truncate(doubleValue);
            }

            return false;
        }
    }

}
