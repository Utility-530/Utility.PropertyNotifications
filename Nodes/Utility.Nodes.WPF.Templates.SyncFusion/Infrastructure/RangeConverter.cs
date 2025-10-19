
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Utility.Nodes.WPF.Templates.SyncFusion
{
    public class RangeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
                return DependencyProperty.UnsetValue;
            else
                return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

}
