using Utility.Interfaces.Generic;
using Utility.Interfaces.NonGeneric;

namespace Utility.Models
{
    public class Connection : IKey<Key>
    {
        public Key Key { get; init; }
        public Key Source { get; init; }
        public Key Target { get; init; }

        public bool Equals(IKey<Key>? other)
        {
            return other?.Key.Equals(this.Key) == true;
        }

        public bool Equals(IEquatable? other)
        {
            return (other as IKey<Key>)?.Key == this.Key;
        }
    }
}