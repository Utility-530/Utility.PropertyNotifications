using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Utility.FileSystem.Transfer.WPF.Controls.Infrastructure
{
    public class BorderClipConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 3 || !(values[0] is double) || !(values[1] is double) || !(values[2] is CornerRadius))
                return (object)Geometry.Empty;
            double num1 = (double)values[0];
            double num2 = (double)values[1];
            if (num1 < double.Epsilon || num2 < double.Epsilon)
                return (object)Geometry.Empty;
            CornerRadius cornerRadius = (CornerRadius)values[2];
            RectangleGeometry rectangleGeometry = new RectangleGeometry(new Rect(0.0, 0.0, num1, num2), cornerRadius.TopLeft, cornerRadius.TopLeft);
            rectangleGeometry.Freeze();
            return (object)rectangleGeometry;
        }

        public object[] ConvertBack(
            object value,
            Type[] targetTypes,
            object parameter,
            CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}