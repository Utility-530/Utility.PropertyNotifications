using System;

namespace Utility.Nodify.Core
{
    public class Diagram
    {
        public Diagram() { }
        public string Name { get; set; }    
        public virtual ConnectionViewModel[] Connections { get; } = Array.Empty<ConnectionViewModel>();
        public virtual NodeViewModel[] Nodes { get; } = Array.Empty<NodeViewModel>();

        public static Diagram Empty => new Diagram();
    }
}
