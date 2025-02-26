using Utility.Interfaces;

namespace Utility.PropertyDescriptors;
public record PropertyDescriptor(Descriptor Descriptor, object Instance) : BasePropertyDescriptor(Descriptor, Instance)
{
    public override IEnumerable<object> Children
    {
        get
        {
            yield return DescriptorConverter.ToDescriptor(Instance, Descriptor);
        }
    }
}

public abstract record BasePropertyDescriptor(Descriptor Descriptor, object Instance) : MemberDescriptor(Descriptor.PropertyType), IInstance, IPropertyDescriptor
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

    public void RaisePropertyChanged(ref object previousValue, object value, string? propertyName = null)
    {
        if (Descriptor.IsReadOnly == true)
            return;
        base.RaisePropertyChanged(ref previousValue, value);
    }

    public override object Get()
    {

        try
        {
            var value = Descriptor.GetValue(Instance);
            return value;
        }
        catch(NotSupportedException ex)
        {
            return null;
        }
    }

    public override void Set(object? value)
    {
        Descriptor.SetValue(Instance, value);
    }

}


