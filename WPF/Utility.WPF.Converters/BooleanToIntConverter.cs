using System;
using System.Globalization;
using System.Windows.Data;

namespace Utility.WPF.Converters
{
    public class BooleanToIntConverter : IValueConverter
    {
        static BooleanToIntConverter()
        {
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Convert.ToInt32(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Convert.ToBoolean(value);
        }

        public static BooleanToIntConverter Instance => new BooleanToIntConverter();
    }

    public class BooleanToIntInverseConverter : IValueConverter
    {
        static BooleanToIntInverseConverter()
        {
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool b
                ? System.Convert.ToInt32(!b)
                : throw new Exception($"Expected value to be of type boolean, actual type is {value?.GetType()}");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !System.Convert.ToBoolean(value);
        }

        public static BooleanToIntInverseConverter Instance => new BooleanToIntInverseConverter();
    }
}