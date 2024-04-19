using Utility.Interfaces.NonGeneric;

namespace Utility.Keys
{
    public record ValueKey<T> : Key, IEquatable
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

        public override string ToString()
        {
            return Value?.ToString();
        }
    }
}