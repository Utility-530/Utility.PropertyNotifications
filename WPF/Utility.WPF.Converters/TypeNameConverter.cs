using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Utility.Helpers.Reflection;

namespace Utility.WPF.Converters
{
    public class TypeNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value == null)
                    return DependencyProperty.UnsetValue;
                else if (value is Type type)
                    return type?.ToFriendlyName() ?? DependencyProperty.UnsetValue;
                else if (value is string str)
                    if (Type.GetType(str) is Type _type)
                        return Convert(_type, targetType, parameter, culture);
                    else
                        return str;
                return Convert(value.GetType(), targetType, parameter, culture);
            }
            catch
            {
                return DependencyProperty.UnsetValue;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static TypeNameConverter Instance { get; } = new();
    }
}