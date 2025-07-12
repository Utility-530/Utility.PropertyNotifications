using System;
using System.Globalization;
using System.Windows;

namespace Utility.WPF.Converters
{
    public class NullableBooleanToInverseConverter : System.Windows.Data.IValueConverter
    {
        public static NullableBooleanToInverseConverter Instance { get; } = new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool?)
            {
                switch ((bool?)value)
                {
                    case null:
                        return null;
                    case false:
                        return true;
                    case true:
                        return false;
                }
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool?)
            {
                switch ((bool?)value)
                {
                    case null:
                        return null;
                    case false:
                        return true;
                    case true:
                        return false;
                }
            }

            return DependencyProperty.UnsetValue;

        }
    }
}
