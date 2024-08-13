
using System.Diagnostics;
using Utility.Interfaces;

namespace Utility.Descriptors;

public interface ICollectionDetailsDescriptor
{
    bool IsCollection { get; }
    Type CollectionItemPropertyType { get; }
    bool IsCollectionItemValueType { get; }
}

public abstract record MemberDescriptor(Type Type) : NotifyProperty, IDescriptor, IIsReadOnly, IChildren, ICollectionDetailsDescriptor
{
    public Guid Guid { get; set; }

    public Guid ParentGuid { get; set; }

    public abstract string? Name { get; }

    public abstract Type ParentType { get; }

    public bool IsCollection => Type != null && Type != typeof(string) && typeof(IEnumerable).IsAssignableFrom(Type);

    public virtual Type? CollectionItemPropertyType => Type?.IsArray == true ? Type.GetElementType() : IsCollection ? Type.GenericTypeArguments().SingleOrDefault() : null;

    public bool IsObservableCollection => Type != null && typeof(INotifyCollectionChanged).IsAssignableFrom(Type);

    public bool IsFlagsEnum => Type?.IsFlagsEnum() == true;

    public bool IsValueType => Type?.IsValueType == true;

    public virtual bool IsCollectionItemValueType => CollectionItemPropertyType != null && CollectionItemPropertyType.IsValueType;

    public abstract bool IsReadOnly { get; }
    public abstract IObservable<object> Children { get; }

    public virtual bool Equals(MemberDescriptor? other) => this.Guid.Equals(other?.Guid);

    public override int GetHashCode() => this.Guid.GetHashCode();

    public virtual void Initialise(object? item = null)
    {
        VisitChildren(this, a =>
        {
            if (a is IDescriptor descriptor)
            {
                descriptor.Initialise();
            }
        });
    }

    public virtual void Finalise(object? item = null)
    {
        VisitChildren(this, a =>
        {
            if (a is IDescriptor descriptor)
            {
                descriptor.Finalise();
            }
        });
    }
    static void VisitChildren(IChildren tree, Action<object> action)
    {
        tree.Children
            .Cast<Change<IDescriptor>>()
            .Subscribe(a =>
            {
                if (a.Type == Changes.Type.Add)
                {
                    Trace.WriteLine(a.Value.ParentType + " " + a.Value.Type?.Name + " " + a.Value.Name);
                    action(a.Value);
                }
                else
                {
                }
            }, e =>
            {
                throw e;
            });
    }
}


public abstract record ValueMemberDescriptor(Type Type) : MemberDescriptor(Type), IValueDescriptor
{
    public virtual object? Value
    {
        get
        {
            var value = Get();
            RaisePropertyCalled(value);
            return value;
        }
        set
        {
            Set(value);
            RaisePropertyReceived(value);
        }
    }

    public abstract object? Get();

    public abstract void Set(object? value);


}

public abstract record ChildlessMemberDescriptor(Type Type) : MemberDescriptor(Type)
{
    public override IObservable<object> Children { get; } = Observable.Empty<object>();
}