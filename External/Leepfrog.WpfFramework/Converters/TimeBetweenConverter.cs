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
    public class TimeBetweenConverter :
          IMultiValueConverter
    {
        /// <summary>
        /// Value converter that adds a number of days to the given date
        /// </summary>
        /// <remarks>
        /// 0 = date1
        /// 1 = date2
        /// </remarks>

        private readonly string[] DAY_STRINGS = new string[] { "day", "days" };
        private readonly string[] HOUR_STRINGS = new string[] { "hr", "hrs" };
        private readonly string[] MINUTE_STRINGS = new string[] { "min", "mins" };

        public TimeBetweenConverter()
        {

        }

        private bool _returnRaw;
        private TimeSpan _fallback;
        public TimeBetweenConverter(bool returnRaw)
        {
            _returnRaw = returnRaw;
        }

        public TimeBetweenConverter(bool returnRaw, TimeSpan fallback)
        {
            _returnRaw = returnRaw;
            _fallback = fallback;
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                //-------------------------------------------------------------
                //this.AddLog("TIMEBETWEEN", $"'{values[0]}' and '{values[1]}'");
                //-------------------------------------------------------------
                if ((values[0] == null) || (values[1]==null))
                {
                    //this.AddLog("TIMEBETWEEN", $" - something is null - returns {_fallback}");
                    return _fallback;
                }
                if (!( values[0] is DateTimeOffset date1 ))
                {
                    //this.AddLog("TIMEBETWEEN", $" - date1 is not dto - returns {_fallback}");
                    return _fallback;
                }
                //-------------------------------------------------------------
                //this.AddLog("TIMEBETWEEN", $"PART 2");
                if (!( values[1] is DateTimeOffset date2 ))
                {
                    //this.AddLog("TIMEBETWEEN", $" - date2 is not dto - returns {_fallback}");
                    return _fallback;
                }
                //-------------------------------------------------------------
                //this.AddLog("TIMEBETWEEN", $"PART 3");
                var diff = date2 - date1;
                if ( _returnRaw )
                {
                    //this.AddLog("TIMEBETWEEN", $"PART 4a");
                    if (diff==null)
                    {
                        //this.AddLog("TIMEBETWEEN", $" - diff is null - returns {_fallback}");
                        return _fallback;
                    }
                    //this.AddLog("TIMEBETWEEN", $" - returns {diff}");
                    return diff;
                }
                //-------------------------------------------------------------
                //this.AddLog("TIMEBETWEEN", $"PART 4b");
                if ( diff.TotalDays >= 1 )
                {
                    return formatNumbers(diff.TotalDays, DAY_STRINGS, diff.Hours,HOUR_STRINGS);
                }
                if ( diff.TotalHours >= 1 )
                {
                    return formatNumbers(diff.TotalHours, HOUR_STRINGS, diff.Minutes, MINUTE_STRINGS);
                }
                //-------------------------------------------------------------
                return formatNumbers(diff.TotalMinutes, MINUTE_STRINGS);
                //-------------------------------------------------------------
            }
            catch ( Exception ex )
            {
                this.AddLog("TIMEBETWEEN", $"returns error {ex}");
                return _fallback;
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
