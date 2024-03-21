using Utility.Interfaces;

namespace Utility.Descriptors;
internal abstract record PropertyDescriptor(Descriptor Descriptor, object Instance) : MemberDescriptor(Descriptor.PropertyType), IRaisePropertyChanged, IInstance, IPropertyDescriptor
{

    public override string? Name => Descriptor.Name;

    public override Type ParentType => Descriptor.ComponentType;

    public override bool IsReadOnly => Descriptor.IsReadOnly;



    public override object? Get()
    {
        var value = Descriptor.GetValue(Instance);
        return value;
    }

    public override void Set(object? value)
    {
        Descriptor.SetValue(Instance, value);
    }

    public void RaisePropertyChanged(object value, string? propertyName = null)
    {
        if (Descriptor.IsReadOnly == true)
            return;
        Descriptor.SetValue(Instance, value);
        base.RaisePropertyChanged(nameof(Value));
    }
}


