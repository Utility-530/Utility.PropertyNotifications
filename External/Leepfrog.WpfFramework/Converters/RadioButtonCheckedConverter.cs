using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;

namespace Leepfrog.WpfFramework.Converters
{
    /// <summary>
    /// Helper class for binding with radio buttons
    /// </summary>
    public class RadioButtonCheckedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            return value.ToString() == (string) parameter;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            return value.Equals(true) ? Enum.Parse(targetType, (string) parameter) : Binding.DoNothing;
        }
    }
}
