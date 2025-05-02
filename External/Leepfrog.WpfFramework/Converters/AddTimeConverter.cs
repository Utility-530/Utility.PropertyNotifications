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

using System.ComponentModel.DataAnnotations;

namespace Leepfrog.WpfFramework.Converters
{
    public class AddTimeConverter :
          IMultiValueConverter
    {
        /// <summary>
        /// Value converter that adds a number of days to the given date
        /// </summary>
        /// <remarks>
        /// 0 = date
        /// 1 = number to add
        /// 2 = type of period - day month hour etc
        /// </remarks>

        public enum TimePeriods
        {
            Seconds,
            Minutes,
            Hours,
            Days,
            Weeks,
            Months,
            Years
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                //-------------------------------------------------------------
                Type dateType;
                DateTimeOffset date = DateTimeOffset.MinValue;
                TimeSpan time = TimeSpan.Zero;
                int number;
                TimePeriods period;
                //-------------------------------------------------------------
                if (values[0] is DateTime datetime)
                {
                    date = datetime;
                    dateType = typeof(DateTime);
                }
                else if (values[0] is DateTimeOffset datetimeoffset)
                {
                    date = datetimeoffset;
                    dateType = typeof(DateTimeOffset);
                }
                else if (values[0] is TimeSpan timeIn)
                {
                    time = timeIn;
                    dateType = typeof(TimeSpan);
                }
                else
                {
                    return DependencyProperty.UnsetValue;
                }
                //-------------------------------------------------------------
                if (values[1] is int)
                {
                    number = (int)values[1];
                }
                else
                {
                    var conv = TypeDescriptor.GetConverter(typeof(int));
                    number = (int)conv.ConvertFrom(values[1]);
                }
                //-------------------------------------------------------------
                if (values[2] is string periodString)
                {
                    Enum.TryParse(periodString, out period);
                }
                //-------------------------------------------------------------
                else if (values[2] is TimePeriods timeperiod)
                {
                    period = timeperiod;
                }
                //-------------------------------------------------------------
                else
                {
                    period = TimePeriods.Days;
                }
                //-------------------------------------------------------------
                if (dateType == typeof(TimeSpan))
                {
                    var newTime = time;
                    switch (period)
                    {
                        case TimePeriods.Seconds:
                            newTime += TimeSpan.FromSeconds(number);
                            break;
                        case TimePeriods.Minutes:
                            newTime += TimeSpan.FromMinutes(number);
                            break;
                        case TimePeriods.Hours:
                            newTime += TimeSpan.FromHours(number);
                            break;
                    }
                    return convert(newTime,targetType);
                }
                else
                {
                    var newDate = date;
                    switch (period)
                    {
                        case TimePeriods.Seconds:
                            newDate = date.AddSeconds(number);
                            break;
                        case TimePeriods.Minutes:
                            newDate = date.AddMinutes(number);
                            break;
                        case TimePeriods.Hours:
                            newDate = date.AddHours(number);
                            break;
                        case TimePeriods.Days:
                            newDate = date.AddDays(number);
                            break;
                        case TimePeriods.Weeks:
                            newDate = date.AddDays(number * 7);
                            break;
                        case TimePeriods.Months:
                            newDate = date.AddMonths(number);
                            break;
                        case TimePeriods.Years:
                            newDate = date.AddYears(number);
                            break;
                    }
                    {
                        if (dateType == typeof(DateTimeOffset))
                        {
                            return convert(newDate,targetType);
                        }
                        else if (dateType == typeof(DateTime))
                        {
                            return convert(newDate.LocalDateTime,targetType);
                        }
                        return DependencyProperty.UnsetValue;
                    }
                }
            }
            catch (Exception ex)
            {
                this.AddLog(ex);
                return DependencyProperty.UnsetValue;
                //return ex.Message;
            }

            //return DependencyProperty.UnsetValue;
        }

        private object convert(object value, Type targetType)
        {
            if (targetType.IsInstanceOfType(value))
            {
                return value;
            }

            TypeConverter converter = TypeDescriptor.GetConverter(targetType);

            try
            {
                // determine if the supplied value is of a suitable type
                if (converter.CanConvertFrom(value.GetType()))
                {
                    // return the converted value
                    return converter.ConvertFrom(value);
                }
                else if (value == DependencyProperty.UnsetValue)
                {
                    return value;
                }
                else
                {
                    // try to convert from the string representation
                    return converter.ConvertFrom(value.ToString());
                }
            }
            catch (Exception)
            {
                return value;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
