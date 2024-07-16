using Utility.Interfaces.Generic;

namespace Utility.Changes
{
    public record Change<T> : Change, IValue<T>
    {
        public Change(T? tValue, Type type) : base(tValue, type)
        {
            Value = tValue;
        }

        public new T Value { get; }

        public new Set<T> ToChangeSet()
        {
            return new Set<T>(this);
        }

        public static Change<T> None => new(default, Type.None);

        public Change<TR> As<TR>() where TR : class
        {
            return new Change<TR>(Value as TR ?? throw new Exception("sd ssss"), Type);
        }
    }

    public record Update<T> :Change<T>
    {
        public Update(T? tValue, string property) : base(tValue, Type.Update)
        {
            Property = property;
        }

        public string Property { get; }
    }


}