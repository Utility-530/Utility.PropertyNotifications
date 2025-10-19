using SQLite;
using System;

namespace Utility.Entities
{
    public class Event
    {
        [PrimaryKey]
        public Guid Guid { get; set; }
        public Guid ParentId { get; set; }
        public DateTime Time { get; set; }
        public string Source { get; set; }
        public string Type { get; set; }
    }
}
