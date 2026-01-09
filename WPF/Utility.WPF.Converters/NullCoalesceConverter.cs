using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Utility.WPF.Converters
{
    /// <summary>
    /// Provides a value converter that returns the first non-null and non-UnsetValue input from a set of values. This
    /// is typically used in data binding scenarios to coalesce multiple potential sources into a single value.
    /// </summary>
    /// <remarks>Use NullCoalesceConverter in XAML multi-binding expressions when you want to select the first
    /// available value from several bindings. This converter is especially useful for fallback scenarios where multiple
    /// sources may be null or unset. The converter returns DependencyProperty.UnsetValue if all input values are null
    /// or UnsetValue.</remarks>
    public class NullCoalesceConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            foreach (var value in values)
            {
                if (value != null && value != DependencyProperty.UnsetValue)
                    return value;
            }
            return DependencyProperty.UnsetValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static NullCoalesceConverter Instance { get; } = new();
    }
}
