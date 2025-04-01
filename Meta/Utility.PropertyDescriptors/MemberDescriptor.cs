
using System.Diagnostics;
using Utility.Interfaces;

namespace Utility.PropertyDescriptors;



public interface ICollectionDetailsDescriptor
{
    bool IsCollection { get; }
    Type CollectionItemPropertyType { get; }
    bool IsCollectionItemValueType { get; }
}


public abstract record MemberDescriptor(Descriptor Descriptor) : NotifyProperty, IDescriptor, IIsReadOnly, ICollectionDetailsDescriptor, IAsyncClone, IHasChildren
{
    public virtual string Name => Descriptor.Name;
    public virtual Type ParentType => Descriptor.ComponentType;
    public virtual Type Type => Descriptor.PropertyType;

    public bool IsCollection => Type != null && Type != typeof(string) && typeof(IEnumerable).IsAssignableFrom(Type);

    [JsonIgnore]
    public virtual Type? CollectionItemPropertyType => Type?.IsArray == true ? Type.GetElementType() : IsCollection ? Type.GenericTypeArguments().SingleOrDefault() : null;

    public bool IsObservableCollection => Type != null && typeof(INotifyCollectionChanged).IsAssignableFrom(Type);

    public bool IsFlagsEnum => Type?.IsFlagsEnum() == true;

    public bool IsValueType => Type?.IsValueType == true;

    public virtual bool IsCollectionItemValueType => CollectionItemPropertyType != null && CollectionItemPropertyType.IsValueType;

    public virtual bool IsReadOnly => Descriptor.IsReadOnly;

    [JsonIgnore]
    public abstract IEnumerable Children { get; }

    public virtual bool Equals(MemberDescriptor? other) => this.Name.Equals(other?.Name) && this.Type == other.ParentType;

    public override int GetHashCode() => this.Name.GetHashCode();

    public virtual bool HasChildren { get; } = true;

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
    static void VisitChildren(IYieldChildren tree, Action<object> action)
    {
        tree.Children
            .Cast<IDescriptor>()
            .ForEach(a =>
            {

                Trace.WriteLine(a.ParentType + " " + a.Type?.Name + " " + a.Name);
                action(a);

            });
    }

    public Task<object> AsyncClone()
    {
        return Task.FromResult<object>(this with { });
    }

    public sealed override string ToString()
    {
        return Name;
    }
}

public abstract record ValueMemberDescriptor(Descriptor Descriptor) : MemberDescriptor(Descriptor), IValueDescriptor
{
    [JsonIgnore]
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
            var _value = value;
            Set(value);
            RaisePropertyReceived(value, _value);
        }
    }

    public abstract object? Get();

    public abstract void Set(object? value);

}

public abstract record ChildlessMemberDescriptor(Descriptor Descriptor) : MemberDescriptor(Descriptor)
{
    public override IEnumerable<object> Children { get; } = Array.Empty<object>();

    public override bool HasChildren => false;
}