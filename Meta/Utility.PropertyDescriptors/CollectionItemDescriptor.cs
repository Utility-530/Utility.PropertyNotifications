using Utility.Meta;

namespace Utility.PropertyDescriptors;

internal class CollectionHeadersDescriptor : MemberDescriptor, ICollectionHeadersDescriptor, IEquatable
{
    private DateTime? removed;

    private List<IDescriptor> descriptors = new();

    public CollectionHeadersDescriptor(Type propertyType, Type componentType) : base(new RootDescriptor(propertyType, componentType, "Header"))
    {
    }

    //public override int Index => 0;

    public override bool IsReadOnly => true;

    public override IEnumerable Items()
    {
        if (Type?.GetConstructor(Type.EmptyTypes) == null || Type.IsValueOrString())
            yield break;
        else if (descriptors.IsEmpty())
        {
            foreach (Descriptor descriptor in TypeDescriptor.GetProperties(Type))
            {
                var hd = new HeaderDescriptor(descriptor.PropertyType, descriptor.ComponentType, descriptor.Name) { Inputs = [], Outputs = [] };
                descriptors.Add(hd);
                yield return hd;
            }
        }
        else
        {
            foreach (IDescriptor descriptor in descriptors)
            {
                yield return descriptor;
            }
        }
    }

    public object Item => null;

    public override int Count => descriptors.Count;

    public override bool Equals(IEquatable? other)
    {
        if (other is CollectionHeadersDescriptor collectionHeaderDescriptor)
            return this.Type.Equals(collectionHeaderDescriptor.Type);
        return false;
    }

    public override bool IsSingular => true;

    public override int GetHashCode()
    {
        var x = base.GetHashCode();
        return x;
    }

    public override void Initialise(object? item = null)
    {
    }

    public override void Finalise(object? item = null)
    {
    }
}

internal class HeaderDescriptor<T, TR>(string Name) : HeaderDescriptor(new RootDescriptor(typeof(T), typeof(TR), Name))
{
}

internal class HeaderDescriptor<T>(string Name) : HeaderDescriptor(new RootDescriptor(typeof(T), null, Name))
{
}

internal class HeaderDescriptor : ChildlessMemberDescriptor, IHeaderDescriptor, IGetType
{
    public HeaderDescriptor(Type type, Type parentType, string name) : base(new RootDescriptor(type, parentType, name))
    {
    }

    public HeaderDescriptor(PropertyDescriptor descriptor) : base(descriptor)
    {
    }

    public override bool IsReadOnly => true;

    public override void Initialise(object? item = null)
    {
    }

    public override void Finalise(object? item = null)
    {
    }

    public new Type GetType()
    {
        if (ParentType == null)
        {
            Type[] typeArguments = { Type };
            Type genericType = typeof(HeaderDescriptor<>).MakeGenericType(typeArguments);
            return genericType;
        }
        else
        {
            Type[] typeArguments = { Type, ParentType };
            Type genericType = typeof(HeaderDescriptor<,>).MakeGenericType(typeArguments);
            return genericType;
        }
    }
}