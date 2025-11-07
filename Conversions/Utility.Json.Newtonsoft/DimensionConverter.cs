using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Utility.Enums;
using Utility.Structs;

namespace Utility.Conversions.Json.Newtonsoft
{
    public class DimensionConverter : JsonConverter<Dimension>
    {
        public override bool CanWrite => false;

        public override Dimension ReadJson(JsonReader reader, Type objectType, Dimension existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return default;
            }
            if (reader.Value is string s)
            {
                if (s == "*")
                    return new Dimension(1, DimensionUnitType.Star);
                else if (double.TryParse(s, out double xx))
                {
                    return new Dimension(xx);
                }
                else if (Enum.TryParse(s, out DimensionUnitType x))
                {
                    return new Dimension(1, x);
                }
            }
            // Read the JSON object
            var jObject = JObject.Load(reader);

            double value = 1;
            var propertyName = jObject["Value"]?.ToString();
            if (string.IsNullOrEmpty(propertyName))
            {
                value = double.Parse(propertyName);
            }

            var type = Enum.Parse(typeof(DimensionUnitType), jObject["Type"].ToString());

            return new Dimension(value, (DimensionUnitType)type);
        }

        public override void WriteJson(JsonWriter writer, Dimension value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}