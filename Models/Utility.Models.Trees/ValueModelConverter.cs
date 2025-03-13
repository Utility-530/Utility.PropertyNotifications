using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.ComponentModel;
using System.Collections;
using System.Reflection;
using Utility.Helpers;

namespace Utility.Models.Trees.Converters
{
    public class ValueModelConverter : JsonConverter<ValueModel>
    {

        public override void WriteJson(JsonWriter writer, ValueModel value, JsonSerializer serializer)
        {
            // Begin object
            writer.WriteStartObject();

            writer.WritePropertyName("$type");
            serializer.Serialize(writer, typeof(ValueModel));

            writer.WritePropertyName("Name");
            serializer.Serialize(writer, value.Name);


            if (value.Value != null)
            {
                writer.WritePropertyName("Value.Type");       
                serializer.Serialize(writer, value.Value.GetType());

                writer.WritePropertyName("Value");
                serializer.Serialize(writer, value.Value);

            }
            
            // End object
            writer.WriteEndObject();
        }

        public override ValueModel ReadJson(JsonReader reader, Type objectType, ValueModel existingValue, bool hasExistingValue, JsonSerializer serializer)
        {

            var jObject = JObject.Load(reader);
            var name = jObject["Name"].ToString();
            object? value = null;
            if (jObject.ContainsKey("Value.Type"))
            {
                var type = Type.GetType(jObject["Value.Type"].Value<string>());
                value = jObject["Value"].ToObject(type);
            }
            var valueModel = new ValueModel(value) { Name = name };
            return valueModel;
        }
    }
}
