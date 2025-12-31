using Utility.Interfaces;
using Utility.Meta;

namespace Utility.PropertyDescriptors;


internal class PropertyDescriptor<T>(string Name) : PropertyDescriptor(new RootDescriptor(typeof(T), null, Name), null)
{
}


public class PropertyDescriptor(Descriptor Descriptor, object Instance) : MemberDescriptor(Descriptor, Instance), IInstance, IPropertyDescriptor, IGetType
{
    public override IEnumerable Items()
    {
        var propertyDescriptor = DescriptorConverter.ToValueDescriptor(Descriptor, Instance);
        yield return propertyDescriptor;
    }

    public new Type GetType()
    {
        Type[] typeArguments = { Type };
        Type genericType = typeof(PropertyDescriptor<>).MakeGenericType(typeArguments);
        return genericType;
    }
}