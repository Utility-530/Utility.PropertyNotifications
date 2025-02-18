using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace BreadcrumbLib.Infrastructure
{
    public class PlacementRectangleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is FrameworkElement element)
            {
                return new Rect(element.ActualWidth, element.ActualHeight, 0d, 0d);
            }    
            if (value is Size size)
            {
                return new Rect(size.Width, size.Height, 0d, 0d);
            }
            return new Rect();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static PlacementRectangleConverter Instance { get; } = new PlacementRectangleConverter();
    }
}
