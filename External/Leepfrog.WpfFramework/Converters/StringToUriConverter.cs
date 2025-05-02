using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Leepfrog.WpfFramework.Converters
{
    public class StringToUriConverter : MarkupExtension, IValueConverter
    {
        #region IValueConverter Members
        //****************************************************************************

        [ConstructorArgument("format")]
        public string Format { get; set; }

        public StringToUriConverter(string format)
        {
            Format = format;
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //=================================================================
            if (value is String s)
            {
                var appFolder = AppDomain.CurrentDomain.BaseDirectory;
                s = Format
                    .Replace("$appFolder$", appFolder)
                    .Replace("$value$", s);
                return new Uri(s);
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
