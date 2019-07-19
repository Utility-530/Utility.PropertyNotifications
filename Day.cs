using System;
using System.Collections.Generic;
using System.Text;

namespace UtilityStruct
{ 
    public struct Day
    {
        public Day(int d) { Value = d; }
        public int Value;

        public static implicit operator int(Day d)
        {
            return d.Value;
        }

        public static implicit operator Day(int d)
        {
            return new Day(d);
        }
 
        public static implicit operator Day(DateTime d)
        {
            return new Day((int)(d - default(DateTime)).TotalDays);
        }

        //  User-defined conversion from DateTime to Day 
        public static implicit operator DateTime(Day d)
        {
            return new DateTime((d * TimeSpan.TicksPerDay));
        }

        public override string ToString()
        {
            return ((DateTime)this).ToString("ddd dd MM yy");
        }
    }


}
