namespace Utility.Descriptors;

public abstract record MemberDescriptor(Type Type) : NotifyProperty, IMemberDescriptor
{
    public Guid Guid { get; set; }

    public abstract string? Name { get; }

    public abstract System.Type ParentType { get; }

    //public abstract System.IObservable<Change<IMemberDescriptor>> GetChildren();

    public virtual System.IObservable<Change<IMemberDescriptor>> GetChildren()
    {
        if (Type.GetConstructor(System.Type.EmptyTypes) == null || Type.IsValueOrString())
            return Observable.Empty<Change<IMemberDescriptor>>();

        var instance = Activator.CreateInstance(Type);
        var descriptor = new RootDescriptor(instance);
        return Observable.Create<Change<IMemberDescriptor>>(observer =>
        {
            observer.OnNext(new(descriptor, Changes.Type.Add));
            //observer.OnNext(new(new MethodsDescriptor(descriptor, instance), Changes.Type.Add));
            return Disposable.Empty;
        });
    }

    public abstract object GetValue();

    public abstract void SetValue(object value);

    public bool IsCollection => Type != null && Type != typeof(string) && typeof(IEnumerable).IsAssignableFrom(Type);

    public virtual System.Type? CollectionItemPropertyType => Type?.IsArray == true ? Type.GetElementType() : IsCollection ? Type.GenericTypeArguments().SingleOrDefault() : null;

    public bool IsObservableCollection => Type != null && typeof(INotifyCollectionChanged).IsAssignableFrom(Type);

    public bool IsFlagsEnum => Type?.IsFlagsEnum() == true;

    public bool IsValueType => Type?.IsValueType == true;

    public virtual bool IsCollectionItemValueType => CollectionItemPropertyType != null && CollectionItemPropertyType.IsValueType;

    System.IObservable<object> IMemberDescriptor.Children => GetChildren();

    public virtual bool Equals(MemberDescriptor? other)
    {
        return this.Guid.Equals(other?.Guid);
    }

    public override int GetHashCode()
    {
        return this.Guid.GetHashCode();
    }
}


