using Utility.Interfaces.Generic;

namespace Utility.Changes
{
    public record Change<T> : Change, IGetValue<T>
    {
        public Change(T? tValue, Type type) : this(tValue, default, type)
        {
        }

        public Change(T? tValue, T? oldValue, Type type) : base(tValue, oldValue, type)
        {
            Value = tValue;
            OldValue = oldValue;
        }

        public new T Value { get; }
        public new T OldValue { get; }

        public new Set<T> ToChangeSet()
        {
            return new Set<T>(this);
        }
    }

    public readonly record struct TreeChange<T>(T NewItem, T? OldItem, Type Type, int Level)
    {
        public static TreeChange<T> Add(T NewItem, int level) => new(NewItem, default, Type.Add, level);
        public static TreeChange<T> Remove(T NewItem, int level) => new(NewItem, default, Type.Remove, level);
        public static TreeChange<T> Replace(T NewItem, T OldItem, int level) => new(NewItem, OldItem, Type.Update, level);
    }

    public record Update<T> : Change<T>
    {
        public Update(T? tValue, string property) : base(tValue, Type.Update)
        {
            Property = property;
        }

        public string Property { get; }
    }
}