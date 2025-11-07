using System;

namespace Utility.EventArguments
{
    public class ValueCoercingEventArgs : EventArgs
    {
        public ValueCoercingEventArgs(object newValue, object oldValue)
        {
            NewValue = newValue;
            OldValue = oldValue;
        }

        public object NewValue { get; set; }
        public object OldValue { get; set; }
        public bool Cancel { get; set; } = false;
    }
}