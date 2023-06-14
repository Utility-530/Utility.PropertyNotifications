using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Utility.WPF.Converters
{
    public class IntToEnumConverter : IValueConverter
    {
        public Type? Enum { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (Enum == null)
            {
                throw new NullReferenceException(nameof(StringToEnumConverter));
            }

            int? output = null;
            if (value == null)
                return DependencyProperty.UnsetValue;

            try
            {
                output = (int)value;
            }
            catch
            {
            }
            var xx = output.HasValue ?
                System.Enum.ToObject(Enum, output.Value) :
                int.TryParse(value.ToString(), out int result) ?
                System.Enum.ToObject(Enum, result) :
                DependencyProperty.UnsetValue;
            return xx;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}