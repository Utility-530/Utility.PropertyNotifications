using System;
using System.Linq;
using System.Windows.Data;

namespace Utility.WPF.Converters
{
    public class MultiBooleanValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return values.OfType<bool>().All(System.Convert.ToBoolean);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static MultiBooleanValueConverter Instance => new MultiBooleanValueConverter();
    }

    public class MultiInverseBooleanValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return values.All(System.Convert.ToBoolean);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static MultiInverseBooleanValueConverter Instance => new MultiInverseBooleanValueConverter();
    }
}