
namespace Utility.Descriptors;

public abstract record MemberDescriptor(Type Type) : NotifyProperty, IDescriptor, IIsReadOnly
{
    public Guid Guid { get; set; }

    public Guid ParentGuid { get; set; }

    public abstract string? Name { get; }

    public abstract Type ParentType { get; }


    public virtual object Value {
        get {
            var value = Get();
            RaisePropertyCalled(value);
            return value; 
        }
        set {
            Set(value); 
                RaisePropertyReceived(value); 
        }
    }

    public bool IsCollection => Type != null && Type != typeof(string) && typeof(IEnumerable).IsAssignableFrom(Type);

    public virtual Type? CollectionItemPropertyType => Type?.IsArray == true ? Type.GetElementType() : IsCollection ? Type.GenericTypeArguments().SingleOrDefault() : null;

    public bool IsObservableCollection => Type != null && typeof(INotifyCollectionChanged).IsAssignableFrom(Type);

    public bool IsFlagsEnum => Type?.IsFlagsEnum() == true;

    public bool IsValueType => Type?.IsValueType == true;

    public virtual bool IsCollectionItemValueType => CollectionItemPropertyType != null && CollectionItemPropertyType.IsValueType;

    public abstract bool IsReadOnly { get; }

    public virtual bool Equals(MemberDescriptor? other) => this.Guid.Equals(other?.Guid);

    public override int GetHashCode() => this.Guid.GetHashCode();

    public abstract object Get();

    public abstract void Set(object? value);
}


