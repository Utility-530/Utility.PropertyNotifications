using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Utility.WPF.Helpers;

namespace Utility.WPF.Converters
{
    public class ColorConverter : IValueConverter
    {
        public bool Inverse { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is SolidColorBrush brush)
            {
                value = brush.Color;
            }

            var val = value is System.Drawing.Color color ?
                color.ToMediaColor() :
                value is Color color1 ?
                 color1 :
                 parameter is Color defaultColour ?
                 defaultColour :
                 DependencyProperty.UnsetValue;


            return val is Color col ?
                targetType == typeof(Brush) ? 
                new SolidColorBrush(Invert(col)) :
                Invert(col) :
                val;

            Color Invert(Color col)
            {
                return Inverse ? col.Inverse() : col;
            }

        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static ColorConverter Instance { get; } = new ColorConverter();
    }
}