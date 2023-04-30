using Utility.Interfaces.NonGeneric;
using Utility.Models;

namespace Utility.PropertyTrees.Infrastructure
{
    public record PropertyChange(Key Key, object NewValue, object OldValue) : IValueChange
    {
        public string Name => Key.Name;

        public bool Equals(IEquatable? other)
        {
            return Key.Equals(other);
        }
    }
}