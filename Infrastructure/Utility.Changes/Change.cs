using Utility.Interfaces.NonGeneric;

namespace Utility.Changes
{
    public record Change : IValue
    {
        public Change(object value, object oldValue, Type type, int? count = default)
        {
            Value = value;
            OldValue = oldValue;
            Type = type;
            Count = count;
        }

        public object Value { get; }
        public object OldValue { get; }

        public Type Type { get; init; }
        public int? Count { get; }

        public Set ToChangeSet()
        {
            return new Set(this);
        }
    }
}