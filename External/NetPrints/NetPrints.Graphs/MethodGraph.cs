using NetPrints.Enums;
using NetPrints.Graph;
using NetPrints.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace NetPrints.Core
{
    /// <summary>
    /// Method type. Contains common things usually associated with methods such as its arguments and its name.
    /// </summary>
    [DataContract]
    public partial class MethodGraph : ExecutionGraph, IMethodGraph
    {
        /// <summary>
        /// Return node of this method that when executed will return from the method.
        /// </summary>
        public IEnumerable<IReturnNode> ReturnNodes
        {
            get => Nodes.OfType<IReturnNode>();
        }

        /// <summary>
        /// Main return node that determines the return types of all other return nodes.
        /// </summary>
        public IReturnNode MainReturnNode
        {
            get => Nodes?.OfType<ReturnNode>()?.FirstOrDefault();
        }

        /// <summary>
        /// Ordered return types this method returns.
        /// </summary>
        public IEnumerable<IBaseType> ReturnTypes
        {
            get => MainReturnNode?.InputTypePins?.Select(pin => pin.InferredType ?? TypeSpecifier.FromType<object>())?.ToList() ?? new List<IBaseType>();
        }

        /// <summary>
        /// Generic type arguments of the method.
        /// </summary>
        public IEnumerable<IGenericType> GenericArgumentTypes
        {
            get => EntryNode != null ? EntryNode.OutputTypePins.Select(pin => pin.InferredType).Cast<GenericType>().ToList() : new List<GenericType>();
        }

        /// <summary>
        /// Name of the method without any prefixes.
        /// </summary>
        [DataMember]
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Modifiers this method has.
        /// </summary>
        [DataMember]
        public MethodModifiers Modifiers
        {
            get;
            set;
        } = MethodModifiers.None;

        /// <summary>
        /// Method entry node where this method graph's execution starts.
        /// </summary>
        public IMethodEntryNode MethodEntryNode
        {
            get => (MethodEntryNode)EntryNode;
        }

        /// <summary>
        /// Creates a method given its name.
        /// </summary>
        /// <param name="name">Name for the method.</param>
        public MethodGraph(string name)
        {
            Name = name;
            EntryNode = new MethodEntryNode(this);
            new ReturnNode(this);
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            // Call Node.OnMethodDeserialized until the types don't change anymore
            // or a max iteration was reached.
            // TODO: Sort nodes by depth and propagate in order instead of
            //       doing this inefficient relaxation process.

            int iterations = 0;
            bool anyTypeChanged = true;
            Dictionary<INodeTypePin, IBaseType> pinTypes = new Dictionary<INodeTypePin, IBaseType>();

            while (anyTypeChanged && iterations < 20)
            {
                anyTypeChanged = false;
                pinTypes.Clear();

                foreach (var node in Nodes)
                {
                    node.InputTypePins.ToList().ForEach(p => pinTypes.Add(p, p.InferredType));

                    node.OnMethodDeserialized();

                    if (node.InputTypePins.Any(p => pinTypes[p] != p.InferredType))
                    {
                        anyTypeChanged = true;
                    }
                }

                iterations++;
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
