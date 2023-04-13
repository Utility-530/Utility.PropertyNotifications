using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace Utility.WPF.Animation
{
    internal class BooleanAllConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
           => values.OfType<bool>().All(b => b);

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
           => throw new NotImplementedException();
    }

    internal class DurationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new Duration(TimeSpan.FromSeconds((double)value));
        }

        public object ConvertBack(object value, Type targetTypes, object parameter, CultureInfo culture)
           => throw new NotImplementedException();
    }
}