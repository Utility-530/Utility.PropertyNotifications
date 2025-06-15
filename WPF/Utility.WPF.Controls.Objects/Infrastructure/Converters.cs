using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
