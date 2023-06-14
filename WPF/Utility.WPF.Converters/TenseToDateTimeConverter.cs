using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using Utility.Enums;

namespace Utility.WPF.Converters
{
    public class TenseToDateTimeConverter : IValueConverter
    {
        public int Days { get; set; } = 14;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not Tense tense)
                throw new Exception("Tensefsdfdfsdfsd");

            return tense switch
            {
                Tense.Future => DateTime.Now.GetRange(DateTime.Now.AddDays(Days)).ToList(),
                Tense.Past => DateTime.Now.AddDays(-Days).GetRange(DateTime.Now).ToList(),
                _ => new[] { DateTime.Now },
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Get range of dates between the startdate and enddate
    /// </summary>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <returns></returns>
    internal static class TenseHelper
    {
        public static IEnumerable<DateTime> GetRange(this DateTime startDate, DateTime endDate)
        {
            return Enumerable.Range(0, (int)(endDate - startDate).TotalDays + 1).Select(i => startDate.AddDays(i));
        }
    }
}