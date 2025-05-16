using NetPrints.Interfaces;
//using PropertyChanged;
using System.Runtime.Serialization;
using Utility.PropertyNotifications;

namespace NetPrints.Graph
{
    /// <summary>
    /// Abstract base class for node pins.
    /// </summary>
    [DataContract]
    [KnownType(typeof(NodeInputDataPin))]
    [KnownType(typeof(NodeOutputDataPin))]
    [KnownType(typeof(NodeInputExecPin))]
    [KnownType(typeof(NodeOutputExecPin))]
    [KnownType(typeof(NodeInputTypePin))]
    [KnownType(typeof(NodeOutputTypePin))]
    //[AddINotifyPropertyChangedInterface]
    public abstract class NodePin : NotifyPropertyClass
    {
        /// <summary>
        /// Name of the pin.
        /// </summary>
        [DataMember]
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Node this pin is contained in.
        /// </summary>
        [DataMember]
        public INode Node
        {
            get;
            private set;
        }

        protected NodePin(INode node, string name)
        {
            Node = node;
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
