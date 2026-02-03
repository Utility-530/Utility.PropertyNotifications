namespace Utility.WPF.ComboBoxes.Roslyn
{
    using System.Collections.Generic;

    public sealed class MruTracker
    {
        private readonly Dictionary<object, int> _usage = new();
        private int _counter;

        public void MarkUsed(object item)
        {
            _usage[item] = ++_counter;
        }

        public int GetBoost(object item)
        {
            return _usage.TryGetValue(item, out int t)
                ? -(1000 - t)   // newer = bigger negative (better)
                : 0;
        }
    }


}
