using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Leepfrog.WpfFramework.Converters
{
    public class CaseConverter : MarkupExtension, IValueConverter
    {
        #region IValueConverter Members
        //****************************************************************************

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //=================================================================
            if (value is String)
            {
                return ((string)value).ToUpper();
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
