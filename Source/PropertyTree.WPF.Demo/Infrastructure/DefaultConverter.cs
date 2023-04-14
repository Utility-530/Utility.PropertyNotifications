using System.Globalization;
using System.Windows.Data;

namespace PropertyTrees.WPF.Demo.Infrastructure
{
    public class DefaultConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        public static DefaultConverter Instance { get; } = new DefaultConverter();
    }
}