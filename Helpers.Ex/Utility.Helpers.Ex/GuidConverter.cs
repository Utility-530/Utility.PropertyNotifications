using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Utility.Helpers.Ex
{
    public class GuidConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Guid);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            bool isNullable = Nullable.GetUnderlyingType(objectType) != null;

            if (reader.TokenType == JsonToken.Null)
            {
                if (!isNullable)
                    throw new JsonSerializationException();
                return default;
            }

            var token = JToken.Load(reader);
            if (token.Type == JTokenType.Object)
            {
                return Guid.Parse(token.First.First.Value<string>());
            }
            throw new Exception("s f d 7dgfdfg");
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}