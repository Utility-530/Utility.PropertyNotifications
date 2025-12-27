using Utility.Interfaces.NonGeneric;
using Utility.Meta;

namespace Utility.PropertyDescriptors;

internal class ReferenceDescriptor<T, TR>(string Name) : ReferenceDescriptor(new RootDescriptor(typeof(T), typeof(TR), Name), null)
{
}

internal class ReferenceDescriptor<T>(string Name) : ReferenceDescriptor(new RootDescriptor(typeof(T), null, Name), null)
{
}

internal class ReferenceDescriptor(Descriptor Descriptor, object Instance) : MemberDescriptor(Descriptor, Instance), IReferenceDescriptor, IGetType
{
    public override IEnumerable Items()
    {
        var descriptors = TypeDescriptor.GetProperties(Descriptor.PropertyType);

        foreach (Descriptor descriptor in descriptors)
        {
            yield return DescriptorConverter.ToDescriptor(descriptor, Instance);
        }
    }

    public new Type GetType()
    {
        if (ParentType == null)
        {
            Type[] typeArguments = { Type };
            Type genericType = typeof(ReferenceDescriptor<>).MakeGenericType(typeArguments);
            return genericType;
        }
        else
        {
            Type[] typeArguments = { Type, ParentType };
            Type genericType = typeof(ReferenceDescriptor<,>).MakeGenericType(typeArguments);
            return genericType;
        }
    }
}