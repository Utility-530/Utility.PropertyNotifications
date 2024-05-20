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
                //var headerDescriptor = new HeaderDescriptor(Descriptor.PropertyType, Descriptor.ComponentType, Descriptor.Name);
                //observer.OnNext(new(headerDescriptor, Changes.Type.Add));

                var descriptor = await DescriptorFactory.ToValue(Instance, Descriptor, Guid);
                observer.OnNext(new(descriptor, Changes.Type.Add));
                return Disposable.Empty;
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


    public void RaisePropertyChanged(object value, string? propertyName = null)
    {
        if (Descriptor.IsReadOnly == true)
            return;
        Descriptor.SetValue(Instance, value);
        base.RaisePropertyChanged(nameof(Value));
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


