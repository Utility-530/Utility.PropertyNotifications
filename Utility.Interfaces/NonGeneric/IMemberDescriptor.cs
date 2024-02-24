using System;

namespace Utility.Interfaces.NonGeneric
{
    public interface IMemberDescriptor : IType
    {
        Guid Guid { get; }
        string? Name { get; }
        System.Type ParentType { get; }
        IObservable<object> Children { get; }
        object GetValue();
        void SetValue(object value);

    }
}
