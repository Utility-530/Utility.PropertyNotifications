using System;
using System.Drawing;
using System.Windows;
using System.Windows.Data;

namespace Utility.WPF.Controls.Trees
{
    public class SizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is SizeF size && parameter is string str)
            {
                if (str == "Height")
                {
                    return size.Height > 0 ? size.Height : DependencyProperty.UnsetValue;
                }
                if (str == "Width")
                {
                    return size.Width > 0 ? size.Width : DependencyProperty.UnsetValue;
                }
                else
                {
                    throw new ArgumentException("Parameter must be 'Height' or 'Width'");
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}