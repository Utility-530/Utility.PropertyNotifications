using System;
using System.Collections.Generic;

namespace UtilityInterface.NonGeneric
{
    public interface IPeriodic
    {
        IEnumerable<DateTime> DateTimes { get; }
    }
}
