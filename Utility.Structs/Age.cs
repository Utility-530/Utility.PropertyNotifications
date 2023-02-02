using System;

namespace Utility.Structs
{
    public readonly struct Age
    {
        public Age(int i)
        {
            if(i<0)
            {
                throw new ArgumentException("value must be greate than 0");
            }
            Value = i;
        }

        public double Value { get; }

        public static implicit operator int(Age d)
        {
            return (int)d.Value;
        }


        public static implicit operator double(Age d)
        {
            return (int)d.Value;
        }

        public static implicit operator Age(int d)
        {
            return new Age(d);
        }

        //public static implicit operator Age(TimeSpan d)
        //{
        //    int years=d.T

        //    return new Age(d.Yeat);
        //}

        public Age(double value)
        {
            Value = value;

        }

    }

    public static class TimeSpanExtensions
    {
        public static int GetYears(this TimeSpan timespan)
        {
            return (int)(timespan.Days / 365.2425);
        }
        public static int GetMonths(this TimeSpan timespan)
        {
            return  (int)(timespan.Days / 30.436875);
        }
    }



    ////http://csharphelper.com/blog/2015/02/find-elapsed-time-in-years-months-days-hours-minutes-and-seconds-in-c/
    //// Return the number of years, months, days, hours,
    //// minutes, seconds, and milliseconds you need to add to
    //// from_date to get to_date.
    //private void GetElapsedTime(DateTime from_date, DateTime to_date,
    //    out int years, out int months, out int days, out int hours,
    //    out int minutes, out int seconds, out int milliseconds)
    //{
    //    // If from_date > to_date, switch them around.
    //    if (from_date > to_date)
    //    {
    //        GetElapsedTime(to_date, from_date,
    //            out years, out months, out days, out hours,
    //            out minutes, out seconds, out milliseconds);
    //        years = -years;
    //        months = -months;
    //        days = -days;
    //        hours = -hours;
    //        minutes = -minutes;
    //        seconds = -seconds;
    //        milliseconds = -milliseconds;
    //    }
    //    else
    //    {
    //        // Handle the years.
    //        years = to_date.Year - from_date.Year;

    //        // See if we went too far.
    //        DateTime test_date = from_date.AddMonths(12 * years);
    //        if (test_date > to_date)
    //        {
    //            years--;
    //            test_date = from_date.AddMonths(12 * years);
    //        }

    //        // Add months until we go too far.
    //        months = 0;
    //        while (test_date <= to_date)
    //        {
    //            months++;
    //            test_date = from_date.AddMonths(12 * years + months);
    //        }
    //        months--;

    //        // Subtract to see how many more days,
    //        // hours, minutes, etc. we need.
    //        from_date = from_date.AddMonths(12 * years + months);
    //        TimeSpan remainder = to_date - from_date;
    //        days = remainder.Days;
    //        hours = remainder.Hours;
    //        minutes = remainder.Minutes;
    //        seconds = remainder.Seconds;
    //        milliseconds = remainder.Milliseconds;
    //    }
    //}
}
