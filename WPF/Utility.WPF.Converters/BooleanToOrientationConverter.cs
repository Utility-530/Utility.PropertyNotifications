using System;
using System.Windows.Controls;
using System.Windows.Data;

namespace Utility.WPF.Converters
{
    [ValueConversion(typeof(bool), typeof(Orientation))]
    public class BooleanToOrientationConverter : IValueConverter
    {
        public bool Invert { get; set; }

        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (Invert)
                return (bool)value ? Orientation.Horizontal : Orientation.Vertical;
            else
                return (bool)value ? Orientation.Vertical : Orientation.Horizontal;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}