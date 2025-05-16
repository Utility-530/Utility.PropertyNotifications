using NetPrints.Graph;
using NetPrints.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace NetPrints.Core
{
    /// <summary>
    /// Type graph that returns a type.
    /// </summary>
    [DataContract]
    public class TypeGraph : NodeGraph, ITypeGraph
    {
        /// <summary>
        /// Return node of this type graph that receives the type.
        /// </summary>
        public ITypeReturnNode ReturnNode
        {
            get => Nodes.OfType<TypeReturnNode>().Single();
        }

        /// <summary>
        /// TypeSpecifier for the type this graph returns.
        /// </summary>
        public ITypeSpecifier ReturnType
        {
            get => (TypeSpecifier)(((TypeReturnNode)ReturnNode).TypePin).InferredType ?? TypeSpecifier.FromType<object>();
        }

        public TypeGraph()
        {
            _ = new TypeReturnNode(this);
        }
    }
}
