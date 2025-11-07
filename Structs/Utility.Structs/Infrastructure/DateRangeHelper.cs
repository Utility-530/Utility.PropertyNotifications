using System.Collections.Generic;
using System.Linq;

namespace Utility.Structs
{
    public static class DateRangeHelper
    {
        public static bool HasOverLapWith(this IEnumerable<DateRange> membershipList, DateRange newItem)
        {
            return membershipList.Any(m => m.HasOverLapWith(newItem));
            //return !membershipList.All(m => m.IsFullyAfter(newItem) || newItem.IsFullyAfter(m));
            //return membershipList.Any(m => m.HasPartialOverLapWith(newItem) || newItem.HasFullOverLapWith(newItem));
        }
    }
}