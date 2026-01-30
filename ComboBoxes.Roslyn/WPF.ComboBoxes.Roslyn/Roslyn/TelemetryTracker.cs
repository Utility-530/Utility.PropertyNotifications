using System;
using System.Collections.Generic;
using System.Text;
using System;
using System.Collections.Concurrent;

namespace WPF.ComboBoxes.Roslyn
{

    public sealed class TelemetryTracker
    {
        private readonly ConcurrentDictionary<object, UsageEntry> _usage = new();
        private readonly double _decayPerDay;

        private struct UsageEntry
        {
            public int Count;
            public DateTime LastUsed;
        }

        public TelemetryTracker(double decayPerDay = 0.01)
        {
            _decayPerDay = decayPerDay; // 1% decay per day
        }

        // Called when the user selects a completion
        public void MarkUsed(object item)
        {
            _usage.AddOrUpdate(item,
                _ => new UsageEntry { Count = 1, LastUsed = DateTime.UtcNow },
                (_, old) => new UsageEntry
                {
                    Count = old.Count + 1,
                    LastUsed = DateTime.UtcNow
                });
        }

        // Returns a negative boost for ranking (lower score = higher priority)
        public int GetBoost(object item)
        {
            if (_usage.TryGetValue(item, out var entry))
            {
                var ageDays = (DateTime.UtcNow - entry.LastUsed).TotalDays;
                double decayedCount = entry.Count * Math.Exp(-_decayPerDay * ageDays);
                return -(int)Math.Round(decayedCount);
            }

            return 0;
        }
    }
}
