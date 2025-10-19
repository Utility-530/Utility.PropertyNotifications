using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utility.Entities;
using Utility.Enums;
using Utility.Enums.Attributes;
using Utility.Helpers;

namespace Utility.API.Services.Infrastructure
{
    internal static class Helpers
    {
        public static int AvailableCalls(IEnumerable<DateTime> callHistory, int maxCalls, TimeInterval timeFrame, TimeInterval? outputTimeFrame = default)
        {
            var ratio = 1.0;
            if (outputTimeFrame.HasValue && outputTimeFrame != timeFrame)
            {
                ratio = ToTimeSpan(outputTimeFrame.Value).Ticks / (1d * ToTimeSpan(timeFrame).Ticks);
                DateTime now = DateTime.UtcNow;
                DateTime windowStart = now - ToTimeSpan(outputTimeFrame.Value);
                int usedCalls = callHistory.Count(dt => dt >= windowStart);
                return Math.Max(0, (int)Math.Ceiling(maxCalls * ratio - usedCalls));
            }
            else
            {
                DateTime now = DateTime.UtcNow;
                DateTime windowStart = now - ToTimeSpan(timeFrame);
                int usedCalls = callHistory.Count(dt => dt >= windowStart);
                return Math.Max(0, maxCalls - usedCalls);
            }

            static TimeSpan ToTimeSpan(TimeInterval interval)
            {
                return EnumHelper.GetAttribute<TimeInterval, TimeSpanAttribute>(interval).TimeSpan;
            }
        }

        public static Client InsertClient(this SQLiteConnection connection, string name, int? maxCalls, TimeInterval? timeFrame)
        {
            var guid = Guid.NewGuid();

            // Conditional insert
            var insert = connection.Execute(
                $"INSERT INTO {nameof(Client)} ({nameof(Client.Guid)}, {nameof(Client.Name)}, {nameof(Client.MaxCalls)}, {nameof(Client.MaxCallsTimeFrame)}) " +
                $"SELECT ?, ?, ?, ? " +
                $"WHERE NOT EXISTS (SELECT 1 FROM {nameof(Client)} WHERE {nameof(Client.Name)} = ?);",
                guid, name, maxCalls, timeFrame, name
            );

            // Fetch the inserted or existing client
            return connection.FindWithQuery<Client>(
                $"SELECT * FROM {nameof(Client)} WHERE {nameof(Client.Name)} = ?",
                name
            );
        }

    }
}
