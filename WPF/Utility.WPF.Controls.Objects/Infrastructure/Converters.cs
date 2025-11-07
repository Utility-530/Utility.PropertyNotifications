using Newtonsoft.Json;
using Utility.Conversions.Json.Newtonsoft;
using Utility.WPF.Converters.Json;

namespace Utility.WPF.Controls.Objects
{
    internal class Statics
    {
        public static JsonConverter[] converters = [
    new Newtonsoft.Json.Converters.StringEnumConverter(), new MetadataConverter(), new PointConverter(), new ColorConverter()];
    }
}