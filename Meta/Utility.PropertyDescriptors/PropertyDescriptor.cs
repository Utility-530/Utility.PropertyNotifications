using Utility.Interfaces;

namespace Utility.PropertyDescriptors;

public abstract class BasePropertyDescriptor(Descriptor Descriptor, object Instance) : MemberDescriptor(Descriptor), IInstance, IPropertyDescriptor
{
    public object Instance { get; } = Instance;
}