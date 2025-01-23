using Utility.Interfaces;

namespace Utility.Descriptors;
public record PropertyDescriptor(Descriptor Descriptor, object Instance) : BasePropertyDescriptor(Descriptor,  Instance)
{
    public override IObservable<object> Children
    {
        get
        {
            return Observable.Create<Change<IDescriptor>>(async observer =>
            {
                return DescriptorFactory.ToValue(Instance, Descriptor, Guid)
                .Subscribe(descriptor =>
                {
                    descriptor.Subscribe(changes);
                    observer.OnNext(new(descriptor, Changes.Type.Add));
                });
            });
        }
    }
}

public abstract record BasePropertyDescriptor(Descriptor Descriptor, object Instance) : MemberDescriptor(Descriptor.PropertyType),  IInstance, IPropertyDescriptor
{
    public override string? Name => Descriptor.Name;

    public override Type ParentType => Descriptor.ComponentType;

    public override bool IsReadOnly => Descriptor.IsReadOnly;
}

public abstract record ValuePropertyDescriptor(Descriptor Descriptor, object Instance) : ValueMemberDescriptor(Descriptor.PropertyType), IRaisePropertyChanged, IInstance, IPropertyDescriptor
{
    public override string? Name => Descriptor.Name;

    public override Type ParentType => Descriptor.ComponentType;

    public override bool IsReadOnly => Descriptor.IsReadOnly;


    public void RaisePropertyChanged(ref object previousValue, object value,  string? propertyName = null)
    {
        if (Descriptor.IsReadOnly == true)
            return;
        base.RaisePropertyChanged(ref previousValue, value);
    }

    public override object Get()
    {
        var value = Descriptor.GetValue(Instance);
        return value;
    }

    public override void Set(object? value)
    {
        Descriptor.SetValue(Instance, value);
    }

}


