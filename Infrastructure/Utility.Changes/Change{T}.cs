using Utility.Interfaces.Generic;

namespace Utility.Changes
{
    public record Change<T> : Change, IValue<T>
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

        public static Change<T> None => new(default, Type.None);

        public Change<TR> As<TR>() where TR : class
        {
            return new Change<TR>(Value as TR ?? throw new Exception("sd ssss"), Type);
        }



        public static Change<T> Add(T NewItem) => new Change<T>(NewItem, default, Changes.Type.Add);
        public static Change<T> Remove(T NewItem) => new Change<T>(NewItem, default, Changes.Type.Remove);
        public static Change<T> Update(T NewItem, T OldItem) => new Change<T>(NewItem, OldItem, Changes.Type.Update);



    }


    public readonly record struct TreeChange<T>(T NewItem, T? OldItem, Type Type, int Level)
    {
        public static TreeChange<T> Add(T NewItem, int level) => new TreeChange<T>(NewItem, default, Type.Add, level);
        public static TreeChange<T> Remove(T NewItem, int level) => new TreeChange<T>(NewItem, default, Type.Remove, level);
        public static TreeChange<T> Replace(T NewItem, T OldItem, int level) => new TreeChange<T>(NewItem, OldItem, Type.Update, level);
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