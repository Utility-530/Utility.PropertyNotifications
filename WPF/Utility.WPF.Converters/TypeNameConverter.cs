using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

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
                if (value is Type type)
                    return type?.Name ?? DependencyProperty.UnsetValue;
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