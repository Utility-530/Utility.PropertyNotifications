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
            if (value is SolidColorBrush brush)
            {
                value = brush.Color;
            }
            value = IntToColor(value);
            value = StringToColor(value);
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

        private object IntToColor(object value)
        {
            if (value is Int32)
            {
                var i = (Int32)value;
                return Color.FromRgb(
                        (byte)((i >> 0) & 0xFF),
                        (byte)((i >> 8) & 0xFF),
                        (byte)((i >> 16) & 0xFF)
                        );
            }
            else if (value is Int32?)
            {
                var i = ((Int32?)value).Value;
                return Color.FromRgb(
                        (byte)((i >> 0) & 0xFF),
                        (byte)((i >> 8) & 0xFF),
                        (byte)((i >> 16) & 0xFF)
                        );
            }
            return value;
        }

        private object StringToColor(object value)
        {
            if (value is string s)
            {
                return System.Windows.Media.ColorConverter.ConvertFromString(s);
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Type type = null;

            if (parameter is DataContextSpy { } _dataContextSpy)
            {
                if (_dataContextSpy.DataContext is Utility.Interfaces.NonGeneric.IType _type)
                {
                    type = _type.Type;
                }
            }
            else if (parameter is DataContextSpy { Property: string property } dataContextSpy)
            {
                type = dataContextSpy.DataContext.GetType().GetProperty(property).PropertyType;
            }
            if (type == typeof(Color) && value is Color color)
            {
                return color;
            }
            //if (value is Brush brush)
            //{
            //    return ToHex(brush.Soli);
            //}
            throw new Exception("£ $$$£c£");
        }

        private static String ToHex(System.Drawing.Color c)
    => $"#{c.R:X2}{c.G:X2}{c.B:X2}";

        private static String ToHex(Color c)
=> $"#{c.R:X2}{c.G:X2}{c.B:X2}";

        private static String ToRGB(System.Drawing.Color c)
            => $"RGB({c.R},{c.G},{c.B})";

        public static ColorConverter Instance { get; } = new ColorConverter();
    }
}