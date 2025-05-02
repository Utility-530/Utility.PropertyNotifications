using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;

namespace Leepfrog.WpfFramework.Converters
{
    /// <summary>
    /// Helper class for looking up a resource by its key
    /// </summary>
    public class LookupResourceConverter : IValueConverter
    {
        public string Format { get; set; }

        public LookupResourceConverter()
        {
            Format = "{0}";
        }

        public LookupResourceConverter(string format)
        {
            Format = format;
        }

        #region IValueConverter Members
        //****************************************************************************

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //=================================================================
            var lookup = value;
            //-----------------------------------------------------------------
            if ( (Format != null) && ( value is string) )
            {
                //-----------------------------------------------------------------
                lookup = string.Format(Format, value as string);
                //-----------------------------------------------------------------
            }
            //-----------------------------------------------------------------
            return Application.Current.TryFindResource(lookup);
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
