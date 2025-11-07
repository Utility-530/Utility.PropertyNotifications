using SQLite;
using Utility.Enums;

namespace Utility.Entities
{
    public class Client
    {
        [PrimaryKey]
        public Guid Guid { get; set; }

        [Unique]
        public string Name { get; set; }

        public int MaxCalls { get; set; }

        public TimeInterval MaxCallsTimeFrame { get; set; }
    }
}