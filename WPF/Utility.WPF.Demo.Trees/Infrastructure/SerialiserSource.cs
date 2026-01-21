using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Utility.Conversions.Json.Newtonsoft;
using Utility.Nodes;
using Utility.Helpers.Reflection;

namespace Utility.WPF.Demo.Trees
{
    public class SerialiserSource
    {
        public SerialiserSource()
        {

            Serialiser = new JsonSerializer()
            {
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
        static readonly string[] properties = [ nameof(NodeViewModel.Key),
                nameof(NodeViewModel.IsExpanded),
                nameof(NodeViewModel.Rows),
                nameof(NodeViewModel.Columns),
                nameof(NodeViewModel.Row),
                nameof(NodeViewModel.Column),
                nameof(NodeViewModel.Arrangement),
                nameof(NodeViewModel.Orientation),
                nameof(NodeViewModel.IsSelected),
                nameof(NodeViewModel.IsWithinWindowBounds),
            ];

        public override bool CanRead => base.CanRead;

        public override NodeViewModel? ReadJson(JsonReader reader, Type objectType, NodeViewModel? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, NodeViewModel value, JsonSerializer serializer)
        {
            writer.WriteStartObject();

            var type = value.GetType();

            var filterProperties = type
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(a => properties.Contains(a.Name));

            writer.WritePropertyName(MetadataConverter.Type);
            writer.WriteValue(type.AsString());


            foreach (var prop in filterProperties)
            {
                MetadataConverter.Process(prop, value, writer, serializer);
            }
            writer.WriteEndObject();
        }

    }
}
