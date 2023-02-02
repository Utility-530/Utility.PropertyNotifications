using System;
using System.Windows;
using System.Windows.Data;
using UtilityWpf.Service;

namespace Utility.WPF.Converters
{
    [ValueConversion(typeof(string), typeof(int))]
    public class StringToIntConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return int.TryParse(value.ToString(), out var number) ? number : DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return !(bool)value ? new object() : null;
        }

        public static StringToIntConverter Instance { get; } = new();

    }
}