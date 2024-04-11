using Utility.Interfaces.NonGeneric;

namespace Utility.Keys
{
    public class ValueKey<T> : IEquatable
    {
        public ValueKey(T? value)
        {
            Value = value;
        }

        public bool Equals(IEquatable? other)
        {
            return (other as ValueKey<T>)?.Value?.Equals(Value) == true;
        }

        public T? Value { get; init; }
    }
}