using Utility.Interfaces;
using Utility.Interfaces.Generic;
using Utility.Meta;

namespace Utility.PropertyDescriptors;

internal class ValueDescriptor<T>(Descriptor Descriptor, object Instance) : ValueDescriptor(Descriptor, Instance), IGetValue<T>, IGetType
{
    T IGetValue<T>.Value => Get() is T t ? t : default;

    public ValueDescriptor(string name) : this(new RootDescriptor(typeof(T), null, name), null)
    {
    }

    public new Type GetType()
    {
            Type[] typeArguments = { Type };
            Type genericType = typeof(ValueDescriptor<>).MakeGenericType(typeArguments);
            return genericType;
    }
}

internal abstract class NullableValueDescriptor<T>(Descriptor Descriptor, object Instance) : ValueDescriptor(Descriptor, Instance), IGetValue<T?>
{
    T? IGetValue<T?>.Value => Get() is T t ? t : default;
}

internal class ValueDescriptor(Descriptor Descriptor, object Instance) : ValueMemberDescriptor(Descriptor, Instance), IRaisePropertyChanged, IInstance
{
    public override IEnumerable Items() => Array.Empty<object>();

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

    public bool RaisePropertyChanged(object previousValue, object value, string? propertyName = null)
    {
        if (Descriptor.IsReadOnly == true)
            return false;
        return base.RaisePropertyChanged();
    }
}