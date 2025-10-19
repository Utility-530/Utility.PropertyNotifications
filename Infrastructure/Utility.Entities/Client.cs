using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
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
