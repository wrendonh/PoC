using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace PersistingPoC.Entities
{
    public class CustomIntConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(int?);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jsonValue = serializer.Deserialize<JValue>(reader);

            switch (jsonValue.Type)
            {
                case JTokenType.Float:
                    return (int?)Math.Round(jsonValue.Value<double?>() ?? 0);
                case JTokenType.Integer:
                    return jsonValue.Value<int?>();
                default:
                    throw new FormatException($"Invalid Type: {jsonValue.Type:G}");
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
