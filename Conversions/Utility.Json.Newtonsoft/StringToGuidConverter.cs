using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.Conversions.Json.Newtonsoft
{
    public class StringToGuidConverter : JsonConverter<Guid>
    {

        public override Guid ReadJson(JsonReader reader, Type objectType, Guid existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String)
            {

                if (Guid.TryParse(reader.ReadAsString(), out var guid))
                {
                    return guid;
                }
            }

            return Guid.Empty;
        }


        public override void WriteJson(JsonWriter writer, Guid value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }
    }
}
