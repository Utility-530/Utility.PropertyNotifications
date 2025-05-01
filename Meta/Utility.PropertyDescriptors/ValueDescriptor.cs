using Utility.Interfaces;
using Utility.Interfaces.Generic;
using Utility.Meta;

namespace Utility.PropertyDescriptors;

internal record ValueDescriptor<T, TR>(string name) : ValueDescriptor<T>(new RootDescriptor(typeof(T), typeof(TR), name), null)
{
}

internal record ValueDescriptor<T>(Descriptor Descriptor, object Instance) : ValueDescriptor(Descriptor, Instance), IValue<T>, IGetType
{
    T IValue<T>.Value => Get() is T t ? t : default;

    public ValueDescriptor(string name) : this(new RootDescriptor(typeof(T), null, name), null)
    {
    }

    public new Type GetType()
    {
        if (ParentType == null)
        {
            Type[] typeArguments = { Type };
            Type genericType = typeof(ValueDescriptor<>).MakeGenericType(typeArguments);
            return genericType;
        }
        else
        {
            Type[] typeArguments = { Type, ParentType };
            Type genericType = typeof(ValueDescriptor<,>).MakeGenericType(typeArguments);
            return genericType;
        }
    }
}

internal abstract record NullableValueDescriptor<T>(Descriptor Descriptor, object Instance) : ValueDescriptor(Descriptor, Instance), IValue<T?>
{
    T? IValue<T?>.Value => Get() is T t ? t : default;

}

internal record ValueDescriptor(Descriptor Descriptor, object Instance) : ValueMemberDescriptor(Descriptor), IRaisePropertyChanged, IInstance
{
    public override IEnumerable<object> Children => Array.Empty<object>();

    public override bool HasChildren => false;

    public override object? Get()
    {
        return Descriptor.GetValue(Instance);
    }

    public override void Set(object? value)
    {
        if (Descriptor.IsReadOnly == false)
        {
            Descriptor.SetValue(Instance, value);
        }
    }

    public void RaisePropertyChanged(object previousValue, object value, string? propertyName = null)
    {
        if (Descriptor.IsReadOnly == true)
            return;
        base.RaisePropertyChanged();
    }
}

