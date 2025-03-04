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

internal record HeaderDescriptor : ChildlessMemberDescriptor, IHeaderDescriptor
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
}

internal partial record CollectionItemDescriptor : ValueDescriptor, ICollectionItemDescriptor, IEquatable, IItem
{

    internal CollectionItemDescriptor(object item, int index, Type componentType) : base(new RootDescriptor(item.GetType(), componentType, item.GetType().Name) { }, item)
    {
        Item = item;

        if (index == 0)
        {
            throw new Exception("Index 0 reserved for header!");
        }
        Index = index;
    }

    public object Item { get; set; }

    public int Index { get; }

    public override object Get()
    {
        return Item;
    }

    public override void Set(object? value)
    {
        if (Item is IList collection)
        {
            collection[Index] = value;
            Item = value;

        }
        throw new NotImplementedException();
    }

    public static int ToIndex(string name) => int.Parse(MyRegex().Matches(name).First().Groups[1].Value);
    public static string FromIndex(string name, int index) => name + $" [{index}]";

    public bool Equals(IEquatable? other)
    {
        if (other is CollectionItemDescriptor collectionItemDescriptor)
            return Item.Equals(collectionItemDescriptor.Item) && Index.Equals(collectionItemDescriptor.Index);
        return false;
    }

    [GeneratedRegex("\\[(\\d*)\\]")]
    private static partial Regex MyRegex();
}


internal partial record CollectionItemReferenceDescriptor : ReferenceDescriptor, ICollectionItemReferenceDescriptor
{
    private int count = 0;
    List<IDescriptor> _descriptors = [];
    public CollectionItemReferenceDescriptor(object item, int index, Type componentType) : base(new RootDescriptor(item.GetType(), componentType) { }, item)
    {
        Item = item;

        if (index == 0)
        {
            throw new Exception("Index 0 reserved for header!");
        }
        Index = index;
    }

    public object Item { get; set; }

    public int Index { get; }

    public int Count => count;

    public override string? Name => Type.Name + $" [{Index}]";

    public static int ToIndex(string name) => int.Parse(MyRegex().Matches(name).First().Groups[1].Value);

    public static string FromIndex(string name, int index) => name + $" [{index}]";


    public override IEnumerable Children
    {
        get
        {
            if (_descriptors.IsEmpty())
            {
                var descriptors = TypeDescriptor.GetProperties(Instance);
                foreach (Descriptor descriptor in descriptors)
                {
                    var d = DescriptorConverter.ToDescriptor(Instance, descriptor);
                    _descriptors.Add(d);
                    count++;
                    yield return d;
                }
            }
            else
            {
                foreach (IDescriptor descriptor in _descriptors)
                {
                    //DescriptorFactory.ToValue(Instance, descriptor)
                    yield return descriptor;
                }
            }
        }
    }

    public bool Equals(IEquatable? other)
    {
        if (other is CollectionItemReferenceDescriptor collectionItemDescriptor)
            return Item.Equals(collectionItemDescriptor.Item) && Index.Equals(collectionItemDescriptor.Index);
        return false;
    }

    [GeneratedRegex("\\[(\\d*)\\]")]
    private static partial Regex MyRegex();
}
