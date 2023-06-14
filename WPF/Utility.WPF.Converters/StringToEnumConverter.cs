using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Utility.WPF.Converters
{
    public class StringToEnumConverter : IValueConverter
    {
        public Type? Enum { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (Enum == null)
            {
                throw new NullReferenceException(nameof(StringToEnumConverter));
            }

            string? output = null;
            if (value == null)
                return DependencyProperty.UnsetValue;

            try
            {
                output = (string)value;
            }
            catch
            {
            }
            var xx = output != null ? System.Enum.Parse(Enum, output) : DependencyProperty.UnsetValue;

            return xx;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}