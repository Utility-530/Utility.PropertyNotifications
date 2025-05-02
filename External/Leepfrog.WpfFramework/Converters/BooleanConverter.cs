using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;

namespace Leepfrog.WpfFramework.Converters
{
    /// <summary>
    /// Helper class for adding Bindings to the opposite of a property's value
    /// </summary>
    public class BooleanConverter : IValueConverter
    {
        public object FalseValue { get; set; }
        public object TrueValue { get; set; }
        public object NullValue { get; set; }

        #region IValueConverter Members
        //****************************************************************************

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //=================================================================
            if (value == null)
            {
                return NullValue;
            }
            else if (((value is bool) || (value is bool?)))
            {
                return ((bool)value) ? TrueValue : FalseValue;
            }
            else if (value is int)
            {
                return ((int)value == 0) ? TrueValue : FalseValue;
            }
            else
            {
                return Binding.DoNothing;
            }
            //=================================================================
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //=================================================================
            // NOT IMPLEMENTED
            return null;
            //=================================================================
        }

        //****************************************************************************
        #endregion
    }
}
