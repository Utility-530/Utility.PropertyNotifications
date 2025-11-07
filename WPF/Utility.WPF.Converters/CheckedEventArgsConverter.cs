using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Utility.WPF.Converters
{
    public class CheckedEventArgsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is RoutedEventArgs { Source: CheckBox { DataContext: { } dataContext } source } routedEventArgs)
            {
                return dataContext;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static CheckedEventArgsConverter Instance { get; } = new CheckedEventArgsConverter();
    }
}