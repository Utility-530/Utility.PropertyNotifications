using System.Windows;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Utility.WPF.Converters.Json
{
    public class PointConverter : JsonConverter<Point>
    {
        public override void WriteJson(JsonWriter writer, Point value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("X");
            writer.WriteValue(value.X);
            writer.WritePropertyName("Y");
            writer.WriteValue(value.Y);
            writer.WriteEndObject();
        }

        public override Point ReadJson(JsonReader reader, Type objectType, Point existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject obj = JObject.Load(reader);
            double x = (double)obj["X"];
            double y = (double)obj["Y"];
            return new Point(x, y);
        }
    }
}