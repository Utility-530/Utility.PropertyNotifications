using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.Attributes
{
    public class ParamAttribute(bool listenToCollectionChanges = false, int ratePerMinute = 60) : Attribute
    {
        public bool ListenToCollectionChanges { get; } = listenToCollectionChanges;
        public int RatePerMinute { get; } = ratePerMinute;
    }
}
