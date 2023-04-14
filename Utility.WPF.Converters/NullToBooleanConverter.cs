using System;
using System.Windows.Data;

namespace Utility.WPF.Converters
{
    [ValueConversion(typeof(object), typeof(bool))]
    public class NullToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            return value == null;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            return !(bool)value ? new object() : null;
        }

        public static NullToBooleanConverter Instance => new();
    }

    [ValueConversion(typeof(object), typeof(bool))]
    public class InverseNullToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            return value != null;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            return (bool)value ? new object() : null;
        }

        public static InverseNullToBooleanConverter Instance { get; } = new();
    }
}