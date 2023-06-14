using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Utility.WPF.Converters
{
    public class ActualWidthConverter : IMultiValueConverter
    {
        private const int ScrollBarWidth = 25;

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.Length >= 1 && values[0] is double val)
            {
                if (values.Length == 2 && values[1] is ScrollViewer { IsVisible: true })
                {
                    return val - ScrollBarWidth;
                }
                return val;
            }
            return DependencyProperty.UnsetValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static ActualWidthConverter Instance => new();
    }
}