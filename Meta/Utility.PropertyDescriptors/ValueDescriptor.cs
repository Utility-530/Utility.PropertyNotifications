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

internal record ValueDescriptor(Descriptor Descriptor, object Instance) : ValuePropertyDescriptor(Descriptor, Instance)
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

    public override void Initialise(object? item = null)
    {
        //Get();
        //if (value == null)
        //{
        //    value = Descriptor.GetValue(Instance);
        //    repo.Set(Guid, value, DateTime.Now);
        //}
        //if (Descriptor.IsReadOnly == false)
        //{
        //    (dateValue ??= repo.Get(Guid))
        //        .Subscribe(a =>
        //    {
        //        if (a is { Value: { } _value } x)
        //        {
        //            RaisePropertyChanged(_value);
        //        }
        //    });
        //}
    }

    public override void Finalise(object? item = null)
    {
        //var value = Descriptor.GetValue(Instance);
        //if (value != null)
        //    repo.Set(Guid, nameof(Value), value, DateTime.Now);
    }
}



