using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Utility.Nodify.Entities;

namespace Utility.Nodify.Core
{
    public class Diagram
    {
        public ICollection<Node> Nodes { get; set; } = new Collection<Node>();
        public ICollection<Connection> Connections { get; set; } = new Collection<Connection>();
    }
}
