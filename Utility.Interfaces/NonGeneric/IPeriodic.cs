using System;
using System.Collections.Generic;

namespace Utility.Interfaces.NonGeneric
{
    public interface IPeriodic
    {
        IEnumerable<DateTime> DateTimes { get; }
    }
}
