using System.Diagnostics;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Utility.Enums;
using Utility.Helpers;
using Utility.Helpers.NonGeneric;
using Utility.Helpers.Reflection;
using Utility.Interfaces.Exs;
using Utility.Interfaces.NonGeneric;
using Utility.Keys;
using Utility.Structs;
using Utility.ServiceLocation;
using Utility.Reactives;
using Utility.Interfaces;

namespace Utility.Nodes
{
    public class NodeConverter : JsonConverter<NodeViewModel>
    {
        private FieldInfo fieldInfo;

        public override void WriteJson(JsonWriter writer, NodeViewModel value, JsonSerializer serializer)
        {
            // Begin object
            writer.WriteStartObject();

            writer.WritePropertyName("$type");
            writer.WriteValue(value.GetType().AsString());

            writer.WritePropertyName("Key");
            JToken.FromObject(value.Key).WriteTo(writer);

            writer.WritePropertyName("IsExpanded");
            writer.WriteValue(value.IsExpanded);

            writer.WritePropertyName("Rows");
            JToken.FromObject(value.Rows).WriteTo(writer);

            writer.WritePropertyName("Columns");
            JToken.FromObject(value.Columns).WriteTo(writer);

            writer.WritePropertyName("Row");
            writer.WriteValue(value.Row);

            writer.WritePropertyName("Column");
            writer.WriteValue(value.Column);

            writer.WritePropertyName("Arrangement");
            writer.WriteValue(value.Arrangement);

            writer.WritePropertyName("Orientation");
            writer.WriteValue(value.Orientation);

            if (value.Current != null)
            {
                writer.WritePropertyName("Current");
                writer.WriteValue((value.Current as IGetKey).Key);
            }

            if (value.Data != null)
            {
                writer.WritePropertyName("Data");
                var jObject = JToken.FromObject(value.Data, serializer);
                jObject.WriteTo(writer);
            }
            if (value.Children.Any() && value.Data is not IBreadCrumb)
            {
                writer.WritePropertyName("Items");
                JArray jArray = [];  // Create a JArray to hold the items

                int index = 0;
                foreach (var item in value.Children)
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
                //writer.WritePropertyName("Current");
                //var jObjectkey = JToken.FromObject(value.Current);
                //jObjectkey.WriteTo(writer);
            }

            writer.WritePropertyName($"$isenum");
            serializer.Serialize(writer, new[] { nameof(NodeViewModel.Arrangement), nameof(NodeViewModel.Orientation) });

            writer.WriteEndObject();
        }

        public override NodeViewModel ReadJson(JsonReader reader, Type objectType, NodeViewModel? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }
            // Read the JSON object

            JObject jObject = JObject.Load(reader);

            NodeViewModel node = null;
            if (jObject.TryGetValue("Data", StringComparison.InvariantCultureIgnoreCase, out var data))
            {
                var type = Type.GetType(data["$type"].ToString());
                Trace.WriteLine(type);
                var _data = data.ToObject(type, serializer);
                node = new NodeViewModel(_data) { };
            }
            else
            {
                node = new NodeViewModel() { };
            }

            node.Key = new GuidKey(Guid.Parse(jObject["Key"].ToString()));

            node.IsExpanded = bool.Parse(jObject["IsExpanded"].ToString());

            if (jObject["Row"] is JToken rToken)
                node.Row = int.Parse(rToken.ToString());

            if (jObject["Column"] is JToken cToken)
                node.Column = int.Parse(cToken.ToString());

            if (jObject["Arrangement"] is JToken aToken)
                node.Arrangement = Enum.Parse<Arrangement>(aToken.ToString());

            if (jObject["Orientation"] is JToken oToken)
                node.Orientation = Enum.Parse<Orientation>(jObject["Orientation"].ToString());

            if (jObject["Rows"] is JArray rows)
            {
                foreach (var item in rows)
                {
                    var dimension = item.ToObject<Dimension>(serializer);
                    node.Rows.Add(dimension);
                }
            }

            if (jObject["Columns"] is JArray columns)
            {
                foreach (var item in columns)
                {
                    var dimension = item.ToObject<Dimension>(serializer);
                    node.Columns.Add(dimension);
                }
            }

            if (jObject["Items"] is JArray items)
            {
                foreach (var item in items)
                {
                    var _node = item.ToObject<NodeViewModel>(serializer);
                    _node.Parent = node;
                    node.Add(_node);
                }
            }

            if (jObject.ContainsKey("Current"))
            {
                var key = new GuidKey(Guid.Parse(jObject["Current"].ToString()));
                (Utility.Globals.Resolver.Resolve<INodeSource>()).Nodes.AndAdditions().Subscribe(current =>
                {
                    //node.Current = current;
                    if (current.Key().Equals(key))
                        node.Set(current, nameof(NodeViewModel.Current));
                });
            }
            return node;
        }
    }

    public class CustomSerializationBinder : ISerializationBinder
    {
        public Type BindToType(string assemblyName, string typeName)
        {
            if (typeName == "System.Reflection.PropertyInfo")
            {
                // Return PropertyInfo type so your converter can handle it
                return typeof(PropertyInfo);
            }

            return Type.GetType($"{typeName}, {assemblyName}");
        }

        public void BindToName(Type serializedType, out string assemblyName, out string typeName)
        {
            assemblyName = serializedType.Assembly.FullName;
            typeName = serializedType.FullName;
        }
    }
}