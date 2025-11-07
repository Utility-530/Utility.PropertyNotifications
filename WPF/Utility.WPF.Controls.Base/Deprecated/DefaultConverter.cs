using System;
using System.Globalization;
using System.Windows.Data;

namespace Utility.WPF.Controls.Base
{
    internal class DefaultConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static DefaultConverter Instance { get; set; }
    }
}