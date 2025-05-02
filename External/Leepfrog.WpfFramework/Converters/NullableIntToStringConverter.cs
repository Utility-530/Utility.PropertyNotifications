using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;

namespace Leepfrog.WpfFramework.Converters
{
    /// <summary>
    /// Helper class for showing a control when a binding exists
    /// </summary>
    public class NullableIntToStringConverter : IValueConverter
    {
        //****************************************************************************
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //=================================================================
            if (value == null)
            {
                return "-";
            }
            if ((value.GetType() == typeof(String)) && ((String)value == "-"))
            {
                return "-";
            }
            if (value.GetType() == typeof(int?))
            {
                var i = (int?)value;
                if (!i.HasValue)
                {
                    return " ";
                }
                if (i.Value < 0)
                {
                    return string.Format("- {0}", -i.Value);
                }
                if (i.Value > 0)
                {
                    return string.Format("+ {0}", i.Value);
                }
                return "EVEN";
            }
            var format = "{0}{1}{2}";
            var space = " ";
            if ((parameter as string) == "NoSpace")
            {
                space = "";
            }
            if (value.GetType() == typeof(int))
            {
                var i = (int)value;
                if (i < 0)
                {
                    return string.Format(format,"-",space, -i);
                }
                if (i > 0)
                {
                    return string.Format(format, "+", space, i);
                }
                return "EVEN";
            }
            if (value.GetType() == typeof(double))
            {
                var i = (double)value;
                if (i < 0)
                {
                    return string.Format(format, "-", space, -i);
                }
                if (i > 0)
                {
                    return string.Format(format, "+", space, i);
                }
                return "EVEN";
            }
            return "unknown" + value.GetType().ToString();
            //=================================================================
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //=================================================================
            throw new NotImplementedException();
            //=================================================================
        }

        #endregion
        //****************************************************************************
    }
}
