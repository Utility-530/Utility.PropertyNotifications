using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;

namespace Leepfrog.WpfFramework.Converters
{
    public class VisibilityToBooleanConverter : IValueConverter
    {
        #region IValueConverter Members
        //****************************************************************************

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //=================================================================
            if (value is Visibility)
            {
                return ((Visibility)value) == Visibility.Visible;
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
            if ( !((value is bool) || (value is bool?)) )
            {
                return Binding.DoNothing;
            }
            //-----------------------------------------------------------------
            return ((bool)value) ? Visibility.Visible : Visibility.Collapsed;
            //=================================================================
        }

        //****************************************************************************
        #endregion
    }
}
