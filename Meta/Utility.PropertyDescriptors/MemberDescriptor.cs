namespace Utility.Descriptors;

public abstract record MemberDescriptor(Type Type) : NotifyProperty, IMemberDescriptor
{
    public Guid Guid { get; set; }

    public abstract string? Name { get; }

    public abstract Type ParentType { get; }

    //public abstract System.IObservable<Change<IMemberDescriptor>> GetChildren();

    public abstract IObservable<Change<IMemberDescriptor>> GetChildren();


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


