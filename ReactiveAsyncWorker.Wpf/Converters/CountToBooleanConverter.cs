using System;
using System.Windows.Data;

namespace ReactiveAsyncWorker.Wpf.Converters
{
    [ValueConversion(typeof(object), typeof(bool))]
    public class CountToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (int.TryParse((parameter?.ToString()), out int param))
                return param >= (int)value != Invert;
            return 0 == (int)value != Invert;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public bool Invert { get; set; }

    }
}