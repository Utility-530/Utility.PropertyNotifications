using System;
using System.Linq;
using System.Windows.Data;

namespace Utility.WPF.Converters
{
    [ValueConversion(typeof(Utility.Enums.ProcessState), typeof(bool))]
    public class FirstEnumToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value == ((System.Collections.IEnumerable)parameter).Cast<object>().ToArray()[0];
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            var a = ((System.Collections.IEnumerable)parameter).Cast<object>().ToArray();
            return ((bool)value) ? a[0] : a[1];
        }
    }
}