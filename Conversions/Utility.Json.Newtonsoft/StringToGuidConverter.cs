using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.Conversions.Json.Newtonsoft
{
    public class StringToGuidConverter : JsonConverter<Guid>
    {

        public override void WriteJson(JsonWriter writer, Guid value, JsonSerializer serializer)
        {
            writer.WriteValue(value);
        }

        public override Guid ReadJson(JsonReader reader, Type objectType, Guid existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String)
            {

                if (reader.Read())
                {
                    if (Guid.TryParse(reader.ReadAsString(), out var guid))
                    {
                        return guid;
                    }
                }
            }

            return Guid.Empty;
        }
    }
}
