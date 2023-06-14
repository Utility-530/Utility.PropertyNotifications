using System;
using System.Collections.Generic;
using System.Linq;

namespace Utility.WPF.Controls.Date.Helper
{
    public static class DateHelper
    {
        public static int GetMonthDayCount(int year, int month)
        {
            DateTime dt = DateTime.Now;
            var count = DateTime.DaysInMonth(year, month);
            return count;
        }

        public static IEnumerable<DateTime> VisibleDays(int Month, int Year)
        {
            var day_first = new DateTime(Year, Month, 1);
            var week = day_first.DayOfWeek;

            for (int i = ((int)week + 6) % 7; i > 0; i--)
            {
                yield return day_first.AddDays(-i);
            }

            var day_last = day_first;
            var monthDayCount = GetMonthDayCount(Year, Month);
            for (int i = 1; i <= monthDayCount; i++)
            {
                day_last = new DateTime(Year, Month, i);
                yield return day_last;
            }
            var week_last = day_last.DayOfWeek;

            for (int i = 1; i <= (7 - (int)week_last) % 7; i++)
            {
                yield return day_last.AddDays(i);
            }
        }

        public static IEnumerable<DateTime> PlusMinusDateRange(this DateTime date, int deviation = 10)
        {
            return Enumerable.Range(0, (deviation * 2) + 1)
                .Select(a => date.AddDays(-deviation + a));
        }
    }
}
