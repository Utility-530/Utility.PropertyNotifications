using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Leepfrog.WpfFramework.Converters
{
    /// <summary>
    /// Helper class for adding Bindings to the opposite of a property's value
    /// </summary>
    public class BooleanToVisibilityConverter : MarkupExtension, IValueConverter
    {
        #region IValueConverter Members
        //****************************************************************************

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //=================================================================
            if (value == null)
            {
                return Visibility.Collapsed;
            }
            else if (((value is bool) || (value is bool?)))
            {
                return ((bool)value) ? Visibility.Visible : Visibility.Collapsed;
            }
            else if (value is int)
            {
                return ((int)value != 0) ? Visibility.Visible : Visibility.Collapsed;
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
            if (!((value is Visibility) || (value is Visibility?)))
            {
                return Binding.DoNothing;
            }
            //-----------------------------------------------------------------
            return ((Visibility)value) == Visibility.Visible;
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
