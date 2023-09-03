using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Utility.WPF.Converters
{
    [ValueConversion(typeof(System.Drawing.Color), typeof(Color))]
    public class ColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = value is System.Drawing.Color color ?
                color.ToUIColor() :
                value is System.Windows.Media.Color?
                 (System.Windows.Media.Color)value:
                 parameter is System.Windows.Media.Color defaultColour ?
                 defaultColour :
                 DependencyProperty.UnsetValue;
            return targetType == typeof(Brush) && val is Color col ? new SolidColorBrush(col) : val;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static ColorConverter Instance { get; } = new ColorConverter();
    }

    public static class ColorConverterHelper
    {
        public static Color ToUIColor(this System.Drawing.Color color)
        {
            return Color.FromArgb(color.A, color.R, color.G, color.B);
        }
    }
}