using Utility.Meta;

namespace Utility.PropertyDescriptors;

internal record CollectionHeadersDescriptor : MemberDescriptor, ICollectionHeadersDescriptor, IEquatable
{
    private DateTime? removed;

    List<IDescriptor> descriptors = new();
    public CollectionHeadersDescriptor(Type propertyType, Type componentType) : base(new RootDescriptor(propertyType, componentType, "Header"))
    {
    }

    public int Index => 0;

    public override bool IsReadOnly => true;

    public override IEnumerable<object> Children
    {
        get
        {
            if (Type.GetConstructor(Type.EmptyTypes) == null || Type.IsValueOrString())
                yield break;
            else if (descriptors.IsEmpty())
            {
                foreach (Descriptor descriptor in TypeDescriptor.GetProperties(Type))
                {
                    var hd = new HeaderDescriptor(descriptor.PropertyType, descriptor.ComponentType, descriptor.Name) { };
                    descriptors.Add(hd);
                    yield return hd;
                }
            }
            else
            {
                foreach (Descriptor descriptor in descriptors)
                {
                    yield return descriptor;
                }
            }
        }

    }

    public DateTime? Removed { get => removed; set => removed = value; }

    public object Item => null;

    public int Count => descriptors.Count;

    public bool Equals(IEquatable? other)
    {
        if (other is CollectionHeadersDescriptor collectionHeaderDescriptor)
            return this.Type.Equals(collectionHeaderDescriptor.Type);
        return false;
    }



    public override void Initialise(object? item = null)
    {
    }

    public override void Finalise(object? item = null)
    {
    }

}

internal record HeaderDescriptor<T, TR>(string Name) : ReferenceDescriptor(new RootDescriptor(typeof(T), typeof(TR), Name), null)
{
}

internal record HeaderDescriptor<T>(string Name) : ReferenceDescriptor(new RootDescriptor(typeof(T), null, Name), null)
{
}

internal record HeaderDescriptor : ChildlessMemberDescriptor, IHeaderDescriptor, IGetType
{
    public HeaderDescriptor(Type type, Type parentType, string name) : base(new RootDescriptor(type, parentType, name))
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