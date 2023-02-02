
using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Utility.WPF.Helper;

namespace Utility.WPF.Converters
{
    public class HighestVisualParentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //var type = value?.GetType();
            //if (type is not null && value is FrameworkElement element)
            //{
            //    while ((element.FindParent(type) ?? element) is FrameworkElement parent)
            //    {
            //        element = parent;
            //    }
            //    return element;
            //}
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        public static HighestVisualParentConverter Instance => new HighestVisualParentConverter();
    }
}