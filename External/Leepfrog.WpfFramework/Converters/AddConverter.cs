using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;

namespace Leepfrog.WpfFramework.Converters
{
    /// <summary>
    /// Helper class for adding Bindings to the opposite of a property's value
    /// </summary>
    public class AddConverter : IValueConverter
    {
        #region IValueConverter Members
        //****************************************************************************

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //=================================================================
            //int val = 0;
            int par = 0;
            //int.TryParse((string)value, out val);
            int.TryParse((string)parameter, out par);
            var no = (int)value + par;
            string ret;
            if (no < 0)
            {
                ret = no.ToString();
            }
            else if (no > 0)
            {
                ret = "+" + no.ToString();
            }
            else
            {
                ret = "EVEN";
            }
            return ret;
            //=================================================================
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //=================================================================
            return null;
            //=================================================================
        }

        //****************************************************************************
        #endregion
    }
}
