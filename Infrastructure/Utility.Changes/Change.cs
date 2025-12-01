using Utility.Interfaces.NonGeneric;

namespace Utility.Changes
{
    public record Change : IGetValue
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


        public static Change<T> None<T>() => new(default, Type.None);

        public Change<TR> As<TR>() where TR : class
        {
            return new Change<TR>(Value as TR ?? throw new Exception("sd ssss"), Type);
        }

        public static Change<T> Add<T>(T NewItem) => new(NewItem, default, Changes.Type.Add);
        public static Change<T> Remove<T>(T NewItem) => new(NewItem, default, Changes.Type.Remove);
        public static Change<T> Update<T>(T NewItem, T OldItem) => new(NewItem, OldItem, Changes.Type.Update);
        public static Change<T> Reset<T>() => new(default, default, Changes.Type.Reset);
    }
}