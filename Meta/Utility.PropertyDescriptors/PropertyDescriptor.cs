using Utility.Interfaces;

namespace Utility.PropertyDescriptors;
public class PropertyDescriptor(Descriptor Descriptor, object Instance) : MemberDescriptor(Descriptor, Instance), IInstance, IPropertyDescriptor
{
    public override IEnumerable Items()
    {        
        var propertyDescriptor = DescriptorConverter.ToValueDescriptor(Descriptor, Instance);
        //propertyDescriptor.Parent = this;
        yield return propertyDescriptor;
    }
}