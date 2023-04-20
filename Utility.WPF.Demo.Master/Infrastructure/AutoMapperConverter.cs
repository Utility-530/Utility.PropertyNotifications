using System;
using System.Globalization;
using System.Windows.Data;
using Utility.Common;

namespace Utility.WPF.Converters
{
    public class AutoMapperConverter : IValueConverter
    {
        public Type? ToType { get; init; }
        public Type? FromType { get; init; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var mapped = AutoMapperSingleton.Instance.Map(value, value.GetType(), ToType ?? parameter as Type);
            return mapped;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var mapped = AutoMapperSingleton.Instance.Map(value, value.GetType(), FromType ?? parameter as Type);
            return mapped;
        }

        public static AutoMapperConverter Instance { get; } = new AutoMapperConverter();
    }
}