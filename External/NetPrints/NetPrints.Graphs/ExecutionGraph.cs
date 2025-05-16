using NetPrints.Enums;
using NetPrints.Graph;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace NetPrints.Core
{
    [DataContract]
    public abstract class ExecutionGraph : NodeGraph
    {
        /// <summary>
        /// Entry node where execution starts.
        /// </summary>
        [DataMember]
        public IExecutionEntryNode EntryNode
        {
            get;
            protected set;
        }

        /// <summary>
        /// Ordered argument types this graph takes.
        /// </summary>
        public IEnumerable<IBaseType> ArgumentTypes
        {
            get => EntryNode != null ? EntryNode.InputTypePins.Select(pin => pin.InferredType ?? TypeSpecifier.FromType<object>()).ToList() : new List<IBaseType>();
        }

        /// <summary>
        /// Ordered argument types with their names this graph takes.
        /// </summary>
        public IEnumerable<Named<IBaseType>> NamedArgumentTypes
        {
            get => EntryNode != null ? EntryNode.InputTypePins.Zip(EntryNode.OutputDataPins, (type, data) => (type, data))
                .Select(pair => new Named<IBaseType>(pair.data.Name, pair.type.InferredType ?? TypeSpecifier.FromType<object>())).ToList() : new List<Named<IBaseType>>();
        }

        /// <summary>
        /// Visibility of this graph.
        /// </summary>
        [DataMember]
        public MemberVisibility Visibility
        {
            get;
            set;
        } = MemberVisibility.Private;
    }
}
