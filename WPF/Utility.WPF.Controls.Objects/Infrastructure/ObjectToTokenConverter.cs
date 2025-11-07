using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Omu.ValueInjecter;

namespace Utility.WPF.Controls.Objects
{
    public class ObjectToTokenConverter : IValueConverter
    {
        private Dictionary<object, object> properties = new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
                return DependencyProperty.UnsetValue;

            var serialiser = JsonSerializer.CreateDefault(new JsonSerializerSettings { Converters = Statics.converters, TypeNameHandling = TypeNameHandling.All });
            if (parameter is not null)
            {
                properties[value.GetType().GetProperty(parameter.ToString()).GetValue(value).ToString()] = value;
            }
            return JToken.FromObject(value, serialiser);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is JToken token)
            {
                var type = Type.GetType(token["$type"].Value<string>());
                if (parameter is not null)
                {
                    var xx = token[parameter.ToString()].Value<object>().ToString();
                    var x = token.ToObject(type);
                    properties[xx].InjectFrom(x);
                    return properties[parameter];
                }
                return token.ToObject(type);
            }
            throw new NotImplementedException();
        }
    }
}