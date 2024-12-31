using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Reflection;
using System;

namespace Utility.Conversions.Json.Newtonsoft
{
    public class AssemblyJsonConverter : JsonConverter<Assembly>
    {

        public override void WriteJson(JsonWriter writer, Assembly value, JsonSerializer serializer)
        {

            // Begin object
            writer.WriteStartObject();

            writer.WritePropertyName("$type");
            writer.WriteValue("Assembly, System.Reflection");

            // Write each property to JSON
            writer.WritePropertyName("Name");
            writer.WriteValue(value.GetName().Name);

            // End object
            writer.WriteEndObject();
        }

        public override Assembly ReadJson(JsonReader reader, Type objectType, Assembly existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            // Read the JSON object
            JObject jo = JObject.Load(reader);
            return Assembly.Load(jo["Name"].ToString());
        }
    }
}