using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Utility.WPF.Converters
{
    public class ThicknessConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (double.IsNaN((double)value))
                return DependencyProperty.UnsetValue;

            return new Thickness((double)value / -2, 0, 0, 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}