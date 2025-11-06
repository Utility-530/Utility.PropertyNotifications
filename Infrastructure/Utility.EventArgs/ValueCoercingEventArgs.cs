namespace Utility.EventArgs
{
    public class ValueCoercingEventArgs : System.EventArgs
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
