using QuikGraph;
using System.Diagnostics;
using System;

namespace Utility.GraphShapes
{
    /// <summary>
    /// A simple identifiable edge.
    /// </summary>
    [DebuggerDisplay("{" + nameof(Source) + "." + nameof(PocVertex.ID) + "} -> {" + nameof(Target) + "." + nameof(PocVertex.ID) + "}")]
    public class PocEdge : Edge<PocVertex>, IEquatable<PocEdge>
    {
        /// <summary>
        /// Edge ID
        /// </summary>
        //[NotNull]
        public string ID { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PocEdge"/> class.
        /// </summary>
        public PocEdge( string id, PocVertex source, PocVertex target) : base(source, target)
        {
            ID = id;
        }

        public bool Equals(PocEdge? other)
        {
            return (this.ID == other?.ID);
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as PocEdge);
        }
    }
}