using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Utility.WPF.Controls.Objects;

namespace Utility.WPF.Demo.Objects
{
    public class ObjectToTokenConverter : IValueConverter
    {
        private JsonConverter[] converters = [
            new StringToGuidConverter(),
            //new Newtonsoft.Json.Converters.IsoDateTimeConverter(),
            new Newtonsoft.Json.Converters.StringEnumConverter()];


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var serialiser = JsonSerializer.CreateDefault(new JsonSerializerSettings { Converters = converters });
            return JToken.FromObject(value, serialiser);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
