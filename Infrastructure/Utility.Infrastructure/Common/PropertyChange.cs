using Utility.Interfaces.Generic;
using Utility.Interfaces.NonGeneric;
using Utility.Models;

namespace Utility.Infrastructure.Common
{
    public sealed record PropertyChange(Key Key, object NewValue, object OldValue) : IKey<Key>, IValueChange
    {
        public string Name => Key.Name;

        public bool Equals(IEquatable? other)
        {
            return Equals(other as IKey<Key>);
        }

        public bool Equals(IKey<Key>? other)
        {
            return other?.Key.Equals(Key) == true;
        }

        public bool Equals(PropertyChange? other)
        {
            return other?.Key.Equals(Key) == true;
        }

        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }
    }
}