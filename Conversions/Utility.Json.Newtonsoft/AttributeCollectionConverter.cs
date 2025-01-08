using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Utility.Conversions.Json.Newtonsoft
{
    public class AttributeCollectionConverter : JsonConverter<AttributeCollection>
    {
        public override void WriteJson(JsonWriter writer, AttributeCollection value, JsonSerializer serializer)
        {
            writer.WriteStartArray();

            foreach (Attribute attribute in value)
            {
                writer.WriteStartObject();

                writer.WritePropertyName("Type");
                serializer.Serialize(writer, attribute.GetType());

                writer.WritePropertyName("Attribute");
                serializer.Serialize(writer, attribute, attribute.GetType());        

                writer.WriteEndObject();
            }

            writer.WriteEndArray();
        }

        public override AttributeCollection ReadJson(JsonReader reader, Type objectType, AttributeCollection existingValue, bool hasExistingValue, JsonSerializer serializer)
        {   
            return new AttributeCollection(select().ToArray());

            IEnumerable<Attribute> select()
            {
                return from JObject obj in JArray.Load(reader)
                       let type = Type.GetType(obj["Type"].Value<string>())
                       let key = obj["Attribute"].ToString()
                       select ((Attribute)JsonConvert.DeserializeObject(key, type));
            }
        }
    }
}
