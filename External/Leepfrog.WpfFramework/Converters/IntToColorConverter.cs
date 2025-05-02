using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace Leepfrog.WpfFramework.Converters
{
    /// <summary>
    /// Converts Int to Color, parameter says any combination of LRTB for left right top bottom
    /// </summary>
    public class IntToColorConverter : MarkupExtension, IValueConverter
    {

        public IntToColorConverter()
        {

        }

        #region IValueConverter Members
        //****************************************************************************

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //=================================================================
            Color ret;
            Int32 d = 0;
            //-----------------------------------------------------------------
            if (value is Int32) 
            {
                d = (Int32)value;
            }
            else if (value is Int32?)
            {
                d = ((Int32?) value).Value;
            }
            else
            {
                return Binding.DoNothing;
            }
            //-----------------------------------------------------------------
            ret = Color.FromArgb(0xFF, (byte)((d >> 0) & 0xff), (byte)((d >> 8) & 0xff), (byte)((d >> 16) & 0xff));
            return ret;
            //=================================================================
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //=================================================================
            Int32 ret;
            Color d;
            //-----------------------------------------------------------------
            if (value is Color)
            {
                d = (Color)value;
            }
            else if (value is Color?)
            {
                d = ((Color?)value).Value;
            }
            else
            {
                return Binding.DoNothing;
            }
            //-----------------------------------------------------------------
            ret = (d.R << 0) | (d.G << 8) | (d.B << 16);
            return ret;
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
