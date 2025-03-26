using Utility.Interfaces;

namespace Utility.PropertyDescriptors;

public abstract record BasePropertyDescriptor(Descriptor Descriptor, object Instance) : MemberDescriptor(Descriptor), IInstance, IPropertyDescriptor
{

}




