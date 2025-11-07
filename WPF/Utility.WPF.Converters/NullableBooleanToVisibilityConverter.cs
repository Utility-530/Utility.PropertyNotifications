using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Utility.WPF.Converters
{
    public class NullableBooleanToVisibilityConverter : IValueConverter
    {
        public static NullableBooleanToVisibilityConverter Instance { get; } = new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool?)
            {
                switch ((bool?)value)
                {
                    case null:
                        return Visibility.Hidden;

                    case false:
                        return Visibility.Collapsed;

                    case true:
                        return Visibility.Visible;
                }
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}