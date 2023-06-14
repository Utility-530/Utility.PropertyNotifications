using System;
using System.Globalization;
using System.Windows.Data;

namespace Utility.WPF.Converters
{
    public class BooleanToDoubleConverter : IValueConverter
    {
        static BooleanToDoubleConverter()
        {
        }

        public double ValueTrue { get; set; } = 1d;
        public double ValueFalse { get; set; } = 0d;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool b
                ? b ? ValueTrue : ValueFalse
                : throw new Exception($"Expected value to be of type boolean, actual type is {value?.GetType()}");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static BooleanToDoubleConverter Instance => new BooleanToDoubleConverter();
    }
}