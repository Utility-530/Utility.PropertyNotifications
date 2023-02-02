using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Utility.WPF.Converters
{
    public class EnumConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (targetType.FullName == "System.Type")
                {
                    return Enum.Parse(value.GetType(), value?.ToString() ?? throw new NullReferenceException("value is null"));
                }
                return Enum.Parse(targetType, value?.ToString() ?? throw new NullReferenceException("value is null"));
            }
            catch
            {
                return DependencyProperty.UnsetValue;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static EnumConverter Instance { get; } = new EnumConverter();
    }
}