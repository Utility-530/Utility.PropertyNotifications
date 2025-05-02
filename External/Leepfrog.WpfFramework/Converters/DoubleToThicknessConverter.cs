using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Leepfrog.WpfFramework.Converters
{
    /// <summary>
    /// Converts double to thickness, parameter says any combination of LRTB for left right top bottom
    /// </summary>
    [ValueConversion(typeof(Double), typeof(Thickness))]
    public class DoubleToThicknessConverter : MarkupExtension, IValueConverter
    {
        #region IValueConverter Members
        //****************************************************************************

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //=================================================================
            double d = 0;
            string p = "LRTB";
            Thickness ret = new Thickness();
            //-----------------------------------------------------------------
            if (value is double) 
            {
                d = (double) value;
            }
            else if (value is double?)
            {
                d = ((double?) value).Value;
            }
            else if (value is Thickness thickness)
            {
                d = thickness.Top;
            }
            else if (value is IConvertible convertible)
            {
                //return $"TYPE {value.GetType()}";
                d = convertible.ToDouble(culture);
            }
            else
            {
                return Binding.DoNothing;
            }
            //-----------------------------------------------------------------
            if (parameter is string)
            {
                p = ((string)parameter).ToUpper();
            }
            //-----------------------------------------------------------------
            if (p.Contains("L"))
            {
                ret.Left = d;
            }
            if (p.Contains("R"))
            {
                ret.Right = d;
            }
            if (p.Contains("T"))
            {
                ret.Top = d;
            }
            if (p.Contains("B"))
            {
                ret.Bottom = d;
            }
            //-----------------------------------------------------------------
            return ret;
            //=================================================================
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //=================================================================
            return Binding.DoNothing;
            //=================================================================
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        //****************************************************************************
        #endregion
    }
}
