
using Splat;
using Utility.Interfaces.Generic;
using Utility.Repos;

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
    public override string? Name => Descriptor.PropertyType.Name;

    private object? value;
    private readonly ITreeRepository repo = Locator.Current.GetService<ITreeRepository>();
    private IDisposable dateValue;

    public override System.IObservable<object> Children => Observable.Empty<object>();

    public override object? Get()
    {
        if (dateValue == null)
        {
            var previous = value;
            dateValue = repo.Get(Guid, nameof(Value))
                .Subscribe(a =>
            {
                if (a is { Value: { } _value } x)
                {
                    value = _value;
                }
                else if (Descriptor.GetValue(Instance) is { } _val)
                {
                    value = _val;
                    repo.Set(Guid, nameof(Value), _val, DateTime.Now);
                }
                else
                    return;
                changes.OnNext(new(Name, value));
                RaisePropertyChanged(ref previous, value, nameof(Value));
            });
        }
        return value;
    }

    public override void Set(object? value)
    {
        if (Descriptor.IsReadOnly == false)
        {
            repo.Set(Guid, nameof(Value), value, DateTime.Now);
            Descriptor.SetValue(Instance, value);
            this.value = value;
            changes.OnNext(new(Name, value));
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
        var value = Descriptor.GetValue(Instance);
        if (value != null)
            repo.Set(Guid, nameof(Value), value, DateTime.Now);
    }
}



