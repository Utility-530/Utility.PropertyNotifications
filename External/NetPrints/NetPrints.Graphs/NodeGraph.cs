using NetPrints.Graph;
using NetPrints.Interfaces;
using System.Runtime.Serialization;

namespace NetPrints.Core
{
    [DataContract]
    [KnownType(typeof(MethodGraph))]
    [KnownType(typeof(ConstructorGraph))]
    [KnownType(typeof(ClassGraph))]
    [KnownType(typeof(TypeGraph))]
    public abstract class NodeGraph:INodeGraph
    {
        /// <summary>
        /// Collection of nodes in this graph.
        /// </summary>
        [DataMember]
        public IObservableCollection<INode> Nodes
        {
            get;
            private set;
        } = new ObservableRangeCollection<INode>();

        /// <summary>
        /// Class this graph is contained in.
        /// </summary>
        [DataMember]
        public IClassGraph Class
        {
            get;
            set;
        }

        /// <summary>
        /// Project the graph is part of.
        /// </summary>
        public IProject Project
        {
            get;
            set;
        }
    }
}
