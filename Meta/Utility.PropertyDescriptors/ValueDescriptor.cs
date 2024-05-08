
using Splat;
using Utility.Interfaces.Generic;

namespace Utility.Descriptors;


internal abstract record ValueDescriptor<T>(Descriptor Descriptor, object Instance) : ValueDescriptor(Descriptor, Instance), IValue<T>
{

    T IValue<T>.Value => Get() is T t ? t : default;
}

internal abstract record NullableValueDescriptor<T>(Descriptor Descriptor, object Instance) : ValueDescriptor(Descriptor, Instance), IValue<T?>
{

    T? IValue<T?>.Value => Get() is T t ? t : default;
}

internal record ValueDescriptor(Descriptor Descriptor, object Instance) : ValuePropertyDescriptor(Descriptor, Instance)
{
    private object? value;
    private readonly ITreeRepository repo = Locator.Current.GetService<ITreeRepository>();

    public override System.IObservable<object> Children => Observable.Empty<object>();

    public override object? Get()
    {
        if (repo.Get(Guid) is { Value: { } _value } x)
            value = _value;
        else
        {
            value ??= Descriptor.GetValue(Instance);
        }
        return value;
    }

    public override void Set(object? value)
    {
        if (Descriptor.IsReadOnly == false)
        {
            repo.Set(Guid, value, DateTime.Now);
            Descriptor.SetValue(Instance, value);
        }
    }

    public override void Initialise(object? item = null)
    {
        if (Descriptor.IsReadOnly == false)
            if (repo.Get(Guid) is { Value: { } value })
                Descriptor.SetValue(Instance, value);
    }

    public override void Finalise(object? item = null)
    {
        var value = Descriptor.GetValue(Instance);
        if (value != null)
            repo.Set(Guid, value, DateTime.Now);
    }
}



