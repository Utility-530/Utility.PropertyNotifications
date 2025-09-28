using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Utility.Conversions.Json.Newtonsoft;
using Utility.Helpers;
using Utility.Helpers.Reflection;

namespace Utility.Nodes.WPF
{
    public class SerialiserSource
    {
        public SerialiserSource()
        {

            Serialiser = new JsonSerializer() {
                TypeNameHandling = TypeNameHandling.All,
                CheckAdditionalContent = false,          
            };

            Serialiser.Converters.Add(new StringEnumConverter());
            Serialiser.Converters.Add(new NodeConverter());
            Serialiser.Converters.Add(new MetadataConverter());
        }

        public JsonSerializer Serialiser { get; set; }
    }

    public class NodeConverter : JsonConverter<NodeViewModel>
    {
        public override bool CanRead => base.CanRead;

        public override NodeViewModel? ReadJson(JsonReader reader, Type objectType, NodeViewModel? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

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


            writer.WritePropertyName($"$isenum");
            serializer.Serialize(writer, new[] { nameof(NodeViewModel.Arrangement), nameof(NodeViewModel.Orientation) });

            writer.WritePropertyName($"$isreadonly");
            serializer.Serialize(writer, new[] { nameof(NodeViewModel.Key) });

            writer.WriteEndObject();
        }

     
    }
}
