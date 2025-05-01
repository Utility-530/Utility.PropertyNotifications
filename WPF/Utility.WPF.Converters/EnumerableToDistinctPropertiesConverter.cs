using System;
using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Utility.Helpers;
using Utility.Helpers.NonGeneric;
using Utility.Helpers.Reflection;

namespace Utility.WPF.Converters
{
    // Converts enumerable's to a distinct list of given property's (parameter)  value
    [ValueConversion(typeof(IEnumerable), typeof(IEnumerable))]
    public class EnumerableToDistinctPropertiesConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is IEnumerable enumerable && enumerable.Count() > 0)
                return enumerable.Cast<object>()
                        .GetPropertyRefValues<object>((string)parameter)
                        .Distinct()
                        .OrderBy(a => a)
                        .ToList();

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}