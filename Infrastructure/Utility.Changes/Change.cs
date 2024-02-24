using Utility.Interfaces.NonGeneric;

namespace Utility.Changes
{
    public record Change : IValue
    {
        public Change(object value, Type type, int? count = default)
        {
            Value = value;
            Type = type;
            Count = count;
        }

        public object Value { get; }

        public Type Type { get; init; }
        public int? Count { get; }

        public Set ToChangeSet()
        {
            return new Set(this);
        }
    }
}