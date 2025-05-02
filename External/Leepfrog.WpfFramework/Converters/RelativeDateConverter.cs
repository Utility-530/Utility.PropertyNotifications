using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.ComponentModel;

namespace Leepfrog.WpfFramework.Converters
{
    public class RelativeDateConverter :
          IMultiValueConverter
    {
        /// <summary>
        /// Value converter that take a date and today, and compares the two
        /// </summary>
        /// <remarks>
        /// 0 = date1
        /// 1 = date2
        /// </remarks>

        public RelativeDateConverter()
        {

        }

        private bool _returnRaw;
        private TimeSpan _fallback;
        public RelativeDateConverter(bool returnRaw)
        {
            _returnRaw = returnRaw;
        }

        public RelativeDateConverter(bool returnRaw, TimeSpan fallback)
        {
            _returnRaw = returnRaw;
            _fallback = fallback;
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                //-------------------------------------------------------------
                DateTime date1;
                DateTime date2;
                //-------------------------------------------------------------
                if (
                    (values[0] == null)
                 || (values[1] == null)
                   )
                {
                    return null;
                }
                if (values[0] is DateTimeOffset)
                {
                    date1 = ((DateTimeOffset)values[0]).Date;
                }
                else if (values[0] is DateTime)
                {
                    date1 = ((DateTime)values[0]).Date;
                }
                else if (values[0] is DateTimeOffset?)
                {
                    date1 = ((DateTimeOffset?)values[0]).Value.Date;
                }
                else if (values[0] is DateTime?)
                {
                    date1 = ((DateTime?)values[0]).Value.Date;
                }
                else
                {
                    return _fallback;
                }
                //-------------------------------------------------------------
                if (values[1] is DateTimeOffset)
                {
                    date2 = ((DateTimeOffset)values[1]).Date;
                }
                else if (values[1] is DateTime)
                {
                    date2 = ((DateTime)values[1]).Date;
                }
                else if (values[1] is DateTimeOffset?)
                {
                    date2 = ((DateTimeOffset?)values[1]).Value.Date;
                }
                else if (values[1] is DateTime?)
                {
                    date2 = ((DateTime?)values[1]).Value.Date;
                }
                else
                {
                    return _fallback;
                }
                //-------------------------------------------------------------
                var diff = date2 - date1;
                if ( _returnRaw )
                {
                    return diff;
                }
                //-------------------------------------------------------------
                if (date1 == date2)
                {
                    return "Today";
                }
                else if (date1 == (date2.AddDays(-1)))
                {
                    return "Yesterday";
                }
                else if (date1 == (date2.AddDays(1)))
                {
                    return "Tomorrow";
                }
                //-------------------------------------------------------------
                return null;
                //-------------------------------------------------------------
            }
            catch ( Exception ex )
            {
                return ex.Message;
            }

            //return DependencyProperty.UnsetValue;
        }

        private string formatNumbers(double qty1, string[] name1, double qty2=0, string[] name2=null)
        {
            var ret = new List<string>();
            qty1 = Math.Floor(qty1);
            qty2 = Math.Floor(qty2);
            ret.Add($"{qty1} {( qty1 == 1 ? name1[0] : name1[1] )}");
            if ( qty2 > 0 )
            {
                ret.Add($"{qty2} {( qty2 == 1 ? name2[0] : name2[1] )}");
            }
            return string.Join(" ", ret);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
