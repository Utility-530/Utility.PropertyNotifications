using System;
using Utility.Enums;

namespace Utility.Nodes.Filters
{
    public class NodeDTO
    {
        public Guid Guid { get; set; }
        public string Name { get; set; }

        public int? Index { get; set; }

        public string Data { get; set; }

        public bool? IsHighlighted { get; set; }

        public bool IsExpanded { get; set; }

        public Arrangement Arrangement { get; set; }

        public Orientation Orientation { get; set; }

        public int? Rows { get; set; }

        public int? Columns { get; set; }

        public Guid CurrentGuid { get; set; }

    }
}
