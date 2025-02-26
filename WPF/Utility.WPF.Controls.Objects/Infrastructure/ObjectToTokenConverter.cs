using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Utility.WPF.Controls.Objects
{
    public class ObjectToTokenConverter : IValueConverter
    {
        private JsonConverter[] converters = [
            new Newtonsoft.Json.Converters.StringEnumConverter()];


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
                return DependencyProperty.UnsetValue;

            var serialiser = JsonSerializer.CreateDefault(new JsonSerializerSettings { Converters = converters, TypeNameHandling = TypeNameHandling.All });
            return JToken.FromObject(value, serialiser);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is JToken token)
            {
                var type = Type.GetType(token["$type"].Value<string>());
                return token.ToObject(type);
            }
            throw new NotImplementedException();
        }
    }
}
