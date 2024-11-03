namespace Utility.Descriptors;

internal record CollectionHeadersDescriptor : MemberDescriptor, ICollectionHeadersDescriptor, IEquatable
{
    internal CollectionHeadersDescriptor(Type propertyType, Type componentType) : base(propertyType)
    {
        this.ParentType = componentType;
    }

    public override string? Name => "Header";

    public override Type ParentType { get; }

    public int Index => 0;

    public override bool IsReadOnly => true;

    public override IObservable<object> Children
    {
        get
        {
            if (Type.GetConstructor(Type.EmptyTypes) == null || Type.IsValueOrString())
                return Observable.Empty<Change<IDescriptor>>();

            return Observable.Create<Change<IDescriptor>>(observer =>
            {
                foreach (Descriptor descriptor in TypeDescriptor.GetProperties(Type))
                {
                    observer.OnNext(new(new HeaderDescriptor(descriptor.PropertyType, descriptor.ComponentType, descriptor.Name) { Guid =Guid.NewGuid(), ParentGuid = this.Guid }, Changes.Type.Add));
                }
                return Disposable.Empty;
            });
        }

    }

    public DateTime? Removed => null;

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
    public HeaderDescriptor(Type type, Type parentType, string name) : base(type)
    {
        ParentType = parentType;
        Name = name;
    }

    public override string? Name { get; }

    public override Type ParentType { get; }

    public override bool IsReadOnly => true;

    //public override object Value { get => null; set => throw new NotImplementedException(); }


    public override void Initialise(object? item = null)
    {
    }

    public override void Finalise(object? item = null)
    {
    }


}

internal partial record CollectionItemDescriptor : ValueDescriptor, ICollectionItemDescriptor, IEquatable, IItem
{

    internal CollectionItemDescriptor(object item, int index, Type componentType) : base(new RootDescriptor(item.GetType(), componentType) { }, item)
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

    public DateTime? Removed { get; set; }

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


internal partial record CollectionItemReferenceDescriptor :  ReferenceDescriptor, ICollectionItemReferenceDescriptor, IEquatable, IItem
{
    private IObservable<Change<IDescriptor>> observable;

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

    public DateTime? Removed { get; set; }

    public override string? Name => Type.Name;

    public static int ToIndex(string name) => int.Parse(MyRegex().Matches(name).First().Groups[1].Value);

    public static string FromIndex(string name, int index) => name + $" [{index}]";


    public override IObservable<object> Children
    {
        get
        {
            return observable ??= Observable.Create<Change<IDescriptor>>(async observer =>
            {
                var descriptors = TypeDescriptor.GetProperties(Instance);
                foreach (Descriptor descriptor in descriptors)
                {
                    DescriptorFactory.ToValue(Instance, descriptor, Guid)
                    .Subscribe(_descriptor =>
                    {
                        observer.OnNext(new(_descriptor, Changes.Type.Add));
                    });
                }
                return Disposable.Empty;
            });
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
