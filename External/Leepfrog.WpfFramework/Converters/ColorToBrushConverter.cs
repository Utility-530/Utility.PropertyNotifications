using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace Leepfrog.WpfFramework.Converters
{
    /// <summary>
    /// Converts Int to Color
    /// </summary>
    public class ColorToBrushConverter : MarkupExtension, IValueConverter
    {
        public ColorToBrushConverter():base()
        {

        }



        #region IValueConverter Members
        //****************************************************************************

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //=================================================================
            Brush ret;
            Color d;
            //-----------------------------------------------------------------
            if (value == null)
            {
                return Binding.DoNothing;
            }
            //-----------------------------------------------------------------
            if (value is Color)
            {
                d = (Color)value;
            }
            else if (value is Color?)
            {
                d = ((Color?)value).Value;
            }
            else if (value is Int32)
            {
                var i = (Int32)value;
                d = Color.FromRgb(
                        (byte)((i >> 0) & 0xFF),
                        (byte)((i >> 8) & 0xFF),
                        (byte)((i >> 16) & 0xFF)
                        );
            }
            else if (value is Int32?)
            {
                var i = ((Int32?)value).Value;
                d = Color.FromRgb(
                        (byte)((i >> 0) & 0xFF),
                        (byte)((i >> 8) & 0xFF),
                        (byte)((i >> 16) & 0xFF)
                        );
            }
            else
            {
                return Binding.DoNothing;
            }
            //-----------------------------------------------------------------
            ret = new SolidColorBrush(d);
            return ret;
            //=================================================================
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //=================================================================
            return Binding.DoNothing;
            //=================================================================
        }

        //****************************************************************************
        #endregion

        #region MarkupExtension Members
        //****************************************************************************

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            //=================================================================
            return this;
            //=================================================================
        }

        //****************************************************************************
        #endregion
    }
}
