using Utility.Interfaces.NonGeneric;

namespace Utility.Keys
{
    public record StringKey : Key, IEquatable
    {
        public StringKey(string? value)
        {
            Value = value;
        }

        public bool Equals(IEquatable? other)
        {
            return (other as StringKey)?.Value?.Equals(Value) == true;
        }

        public string? Value { get; init; }

        public override string ToString()
        {
            return Value;
        }
    }
}