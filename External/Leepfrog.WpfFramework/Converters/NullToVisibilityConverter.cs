using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;

namespace Leepfrog.WpfFramework.Converters
{
    /// <summary>
    /// Helper class for adding Bindings to the opposite of a property's value
    /// </summary>
    public class NullToVisibilityConverter : IValueConverter
    {
        #region IValueConverter Members
        //****************************************************************************


        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //=================================================================
            if (value == null)
            {
                //-----------------------------------------------------------------
                return Visibility.Visible;
                //-----------------------------------------------------------------
            }
            else
            {
                //-----------------------------------------------------------------
                return Visibility.Collapsed;
                //-----------------------------------------------------------------
            }
            //=================================================================
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //=================================================================
            throw new NotImplementedException();
            //=================================================================
        }

        //****************************************************************************
        #endregion
    }
}
