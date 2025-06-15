using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Utility.WPF.Converters.Json
{
    public class ColorConverter : JsonConverter<Color>
    {
        public override void WriteJson(JsonWriter writer, Color value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("A"); writer.WriteValue(value.A);
            writer.WritePropertyName("R"); writer.WriteValue(value.R);
            writer.WritePropertyName("G"); writer.WriteValue(value.G);
            writer.WritePropertyName("B"); writer.WriteValue(value.B);
            writer.WriteEndObject();
        }

        public override Color ReadJson(JsonReader reader, Type objectType, Color existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject obj = JObject.Load(reader);
            byte a = (byte)obj["A"];
            byte r = (byte)obj["R"];
            byte g = (byte)obj["G"];
            byte b = (byte)obj["B"];
            return Color.FromArgb(a, r, g, b);
        }
    }
}
