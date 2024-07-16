using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Utility.Structs;

namespace Utility.Nodify.Core
{
    public class Node
    {
        public string Name { get; set; }

        public Guid Guid { get; set; }
        public Point Location { get; set; } = new ();
        public string Content { get; set; }
        public ICollection<Connector> Inputs { get; set; } = new Collection<Connector>(); 
        public ICollection<Connector> Outputs { get; set; } = new Collection<Connector>();
    }
}
