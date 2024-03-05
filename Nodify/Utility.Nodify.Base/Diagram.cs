using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace Utility.Nodify.Core
{
    //public class Diagram
    //{
    //    public Diagram() { }
    //    public string Name { get; set; }
    //    public virtual ICollection<ConnectionViewModel> Connections { get; set; } 
    //    public virtual ICollection<NodeViewModel> Nodes { get; set; } 

    //    //public static Diagram Empty => new ();
    //}

    public class Diagram
    {
        public ICollection<Node> Nodes { get; set; } = new Collection<Node>();
        public ICollection<Connection> Connections { get; set; } = new Collection<Connection>();
    }

    public class Node
    {
        public string Name { get; set; }
        public Point Location { get; set; } = new Point();
        public string Content { get; set; }
        public ICollection<Connector> Inputs { get; set; } = new Collection<Connector>(); 
        public ICollection<Connector> Outputs { get; set; } = new Collection<Connector>();
    }

    public class Connector
    {
        public string Key { get; set; }
        public bool IsInput { get; set; }
    }


    public class Connection
    {
        public string Input { get; set; }
        public string Output { get; set; }
    }
}
