namespace Utility.Descriptors;

public record CollectionHeadersDescriptor : MemberDescriptor, ICollectionItemDescriptor, IEquatable, IChildren
{
    public CollectionHeadersDescriptor(Type propertyType, Type componentType) : base(propertyType)
    {
        this.ParentType = componentType;
    }

    public override string? Name => "Header";

    public override Type ParentType { get; }

    public int Index => 0;

    public override object Value { get => null; set => throw new NotImplementedException(); }

    public override bool IsReadOnly => true;

    public IObservable<object> Children
    {
        get
        {
            if (Type.GetConstructor(Type.EmptyTypes) == null || Type.IsValueOrString())
                return Observable.Empty<Change<IDescriptor>>();

            return Observable.Create<Change<IDescriptor>>(observer =>
            {
                foreach (Descriptor descriptor in TypeDescriptor.GetProperties(Type))
                {
                    observer.OnNext(new(new HeaderDescriptor(descriptor.PropertyType, descriptor.ComponentType, descriptor.Name), Changes.Type.Add));
                }
                return Disposable.Empty;
            });
        }

    }


    public bool Equals(IEquatable? other)
    {
        if (other is CollectionHeadersDescriptor collectionHeaderDescriptor)
            return this.Type.Equals(collectionHeaderDescriptor.Type);
        return false;
    }



    public override object? Get()
    {
        return null;
    }

    public override void Set(object? value)
    {
        throw new NotImplementedException();
    }
}

public record HeaderDescriptor : MemberDescriptor
{
    public HeaderDescriptor(Type type, Type parentType, string name) : base(type)
    {
        ParentType = parentType;
        Name = name;
    }

    public override string? Name { get; }

    public override Type ParentType { get; }

    public override bool IsReadOnly => true;

    //public override object Value { get => null; set => throw new NotImplementedException(); }


    public override object? Get()
    {
        return null;
    }

    public override void Set(object? value)
    {
        throw new NotImplementedException();
    }
}

public partial record CollectionItemDescriptor : ValueDescriptor, ICollectionItemDescriptor, IEquatable
{

    public CollectionItemDescriptor(object item, int index, Type componentType) : base(new RootDescriptor(item.GetType(), componentType) { }, item)
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

    public override string? Name => Type.Name;

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


public partial record CollectionItemReferenceDescriptor :  ReferenceDescriptor, ICollectionItemDescriptor, IEquatable
{
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

    public override string? Name => Type.Name;

    public override object Get() => Item;

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
        if (other is CollectionItemReferenceDescriptor collectionItemDescriptor)
            return Item.Equals(collectionItemDescriptor.Item) && Index.Equals(collectionItemDescriptor.Index);
        return false;
    }

    [GeneratedRegex("\\[(\\d*)\\]")]
    private static partial Regex MyRegex();
}
