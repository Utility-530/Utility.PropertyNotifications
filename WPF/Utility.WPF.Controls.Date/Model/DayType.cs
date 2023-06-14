using System;

namespace DateWork.Models
{
    public enum DayType
    {
        Today = 1,
        CurrentMonth = 0,
        OtherMonth = 2
    }

    public static class DayTypeHelper
    {
        public static DayType ToDayType(this DateTime Day)
        {
            var now = DateTime.Now;

            if (Day.Year == now.Year && Day.Month == now.Month)
            {
                if (Day.Day == now.Day)
                {
                    return DayType.Today;
                }
                return DayType.CurrentMonth;
            }
            else
            {
                return DayType.OtherMonth;
            }
        }
    }
}
