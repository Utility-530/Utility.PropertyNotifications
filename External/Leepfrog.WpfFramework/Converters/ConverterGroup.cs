using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using System.Linq;

namespace Leepfrog.WpfFramework.Converters
{
    /// <summary>
    /// Helper class for chaining Bindings together
    /// </summary>
    public class ConverterGroup : List<IValueConverter>, IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //=================================================================
            return this.Aggregate(value, (current, converter) => converter.Convert(current, targetType, parameter, culture));
            //=================================================================
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //=================================================================
            List<IValueConverter> clone = (List<IValueConverter>)this.MemberwiseClone();
            clone.Reverse();
            //-----------------------------------------------------------------
            return this.Aggregate(value, (current, converter) => converter.ConvertBack(current, targetType, parameter, culture));
            //=================================================================
        }

        #endregion
    }
}
