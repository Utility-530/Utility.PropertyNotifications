namespace Utility.Descriptors;

public record CollectionHeadersDescriptor : MemberDescriptor, ICollectionItemDescriptor, IEquatable
{
    public CollectionHeadersDescriptor(Type propertyType, Type componentType) : base(propertyType)
    {
        this.ParentType = componentType;
    }

    public override string? Name => "Header";

    public override Type ParentType { get; }

    public int Index => 0;

    public bool Equals(IEquatable? other)
    {
        if (other is CollectionHeadersDescriptor collectionHeaderDescriptor)
            return this.Type.Equals(collectionHeaderDescriptor.Type);
        return false;
    }

    public override object GetValue()
    {
        throw new NotImplementedException();
    }

    public override void SetValue(object value)
    {
        throw new NotImplementedException();
    }

    public override IObservable<Change<IMemberDescriptor>> GetChildren()
    {
        if (Type.GetConstructor(Type.EmptyTypes) == null || Type.IsValueOrString())
            return Observable.Empty<Change<IMemberDescriptor>>();
      
        return Observable.Create<Change<IMemberDescriptor>>(observer =>
        {
            foreach(Descriptor descriptor in TypeDescriptor.GetProperties(Type))
            {
                observer.OnNext(new(new HeaderDescriptor(descriptor.PropertyType, descriptor.ComponentType, descriptor.Name), Changes.Type.Add));
            }
            return Disposable.Empty;
        });
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

    public override IObservable<Change<IMemberDescriptor>> GetChildren()
    {
        return Observable.Empty<Change<IMemberDescriptor>>();
    }

    public override object GetValue()
    {
        throw new NotImplementedException();
    }

    public override void SetValue(object value)
    {
        throw new NotImplementedException();
    }
}

public partial record CollectionItemDescriptor : PropertyDescriptor, ICollectionItemDescriptor, IEquatable
{
    private readonly Type componentType;

    public CollectionItemDescriptor(object item, int index, Type componentType) : base(new PropertyDescriptor(new RootPropertyDescriptor(item.GetType()) { Item = item}, item))
    {
        Item = item;
        
        if (index == 0)
        {
            throw new Exception("Index 0 reserved for header!");
        }
        Index = index;
        this.componentType = componentType;
    }

    public object Item { get; set; }

    public int Index { get; }

    public override string? Name => Type.Name;

    public override Type ParentType => componentType;

    public override object GetValue()
    {
        return Item;
    }

    public override void SetValue(object? value)
    {
        if (Item is IList collection)
        {
            collection[Index] = value;
            Item = value;
            return;
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


