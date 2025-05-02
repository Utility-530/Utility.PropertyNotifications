using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;

namespace Leepfrog.WpfFramework.Converters
{
    /// <summary>
    /// Helper class for adding Bindings to the opposite of a property's value
    /// </summary>
    public class NotConverter : IValueConverter
    {
        #region IValueConverter Members
        //****************************************************************************

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //=================================================================
            if ( !((value is bool) || (value is bool?)) )
            {
                return Binding.DoNothing;
            }
            //-----------------------------------------------------------------
            return !(bool)value;
            //=================================================================
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //=================================================================
            return Convert(value, targetType, parameter, culture);
            //=================================================================
        }

        //****************************************************************************
        #endregion
    }
}
