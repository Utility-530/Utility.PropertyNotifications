using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Utility.WPF.Converters
{
    public class IntToDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return double.Parse(value.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double d)
            {
                if (double.IsNaN(d))
                {
                    return DependencyProperty.UnsetValue;
                }
            }
            return int.Parse(value.ToString());
        }

        public static IntToDoubleConverter Instance { get; } = new IntToDoubleConverter();
    }
}