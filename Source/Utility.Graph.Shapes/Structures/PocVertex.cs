using Jellyfish;
using System;
using System.Diagnostics;

namespace Utility.GraphShapes
{
    /// <summary>
    /// A simple identifiable vertex.
    /// </summary>
    [DebuggerDisplay("{" + nameof(ID) + "}")]
    public class PocVertex : ObservableObject, IEquatable<PocVertex>
    {
        private int count, broadcast;
        public event Action Visited;

        public PocVertex(string id)
        {
            ID = id ?? throw new ArgumentNullException(nameof(id));
        }

        public string ID { get; }

        public int Count { get => count; set => this.Set(ref count, value); }

        public int Broadcast { get => broadcast; set => this.Set(ref broadcast, value); }

        /// <inheritdoc />
        public override string ToString()
        {
            return ID;
        }

        public bool Equals(PocVertex? other)
        {
            return (this.ID == other?.ID);
        }
        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as PocVertex);
        }
    }
}