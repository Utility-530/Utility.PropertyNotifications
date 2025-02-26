using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Utility.Helpers.NonGeneric;
using Utility.Helpers;
using Utility.Keys;
using Splat;
using Utility.Interfaces.Exs;
using Utility.Interfaces.NonGeneric;

namespace Utility.Nodes
{
    public class NodeConverter : JsonConverter<Node>
    {

        public override void WriteJson(JsonWriter writer, Node value, JsonSerializer serializer)
        {
            // Begin object
            writer.WriteStartObject();

            writer.WritePropertyName("$type");
            writer.WriteValue(value.GetType().AsString());

            writer.WritePropertyName("Key");
            JToken.FromObject(value.Key).WriteTo(writer);

            writer.WritePropertyName("IsExpanded");
            writer.WriteValue(value.IsExpanded);

            writer.WritePropertyName("Data");
            var jObject = JToken.FromObject(value.Data, serializer);
            jObject.WriteTo(writer);

            if (value.Items.Any() && value.Data is not IBreadCrumb)
            {
                writer.WritePropertyName("Items");
                JArray jArray = new JArray();  // Create a JArray to hold the items

                int index = 0;
                foreach (var item in value.Items)
                {
                    // Create a key based on the index or a custom logic
                    //jObject.AddAfterSelf(JToken.FromObject(item));
                    jArray.Add(JToken.FromObject(item, serializer));
                    index++;
                }

                //jObject.AddAfterSelf(jArray);
                //writer.AddFirst(new JProperty("Keys", new JArray(propertyNames)));
                jArray.WriteTo(writer); // Write JObject to the JSON writer
            }
            else if (value.Data is IBreadCrumb breadCrumb && value.Current != null)
            {
                writer.WritePropertyName("Current");
                var jObjectkey = JToken.FromObject(value.Current);
                jObjectkey.WriteTo(writer);
            }

            writer.WriteEndObject();
        }

        public override Node ReadJson(JsonReader reader, Type objectType, Node existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }
            // Read the JSON object

            JObject jObject = JObject.Load(reader);


            var type = Type.GetType(jObject["Data"]["$type"].ToString());
            var data = jObject["Data"].ToObject(type, serializer);

            var node = new Node(data);

            node.Key = new GuidKey(Guid.Parse(jObject["Key"].ToString()));

            var items = jObject["Items"] as JArray;
            if (items != null)
                foreach (var item in items)
                {
                    var _node = item.ToObject<Node>(serializer);
                    node.Add(_node);
                    //NodeEngine.Instance.Add(_node);
                }

            return node;
        }
    }
}
