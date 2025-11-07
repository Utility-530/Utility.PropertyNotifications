using System.Collections.ObjectModel;
using Utility.Interfaces;
using Utility.Interfaces.Exs.Diagrams;
using Utility.Nodes;

namespace Utility.PropertyDescriptors;

public interface ICollectionDetailsDescriptor
{
    bool IsCollection { get; }
    Type CollectionItemPropertyType { get; }
    bool IsCollectionItemValueType { get; }
}

public abstract class MemberDescriptor : NodeViewModel, IDescriptor, IIsReadOnly, ICollectionDetailsDescriptor, IAsyncClone, IHasChildren //, IModel
{
    public MemberDescriptor(Descriptor descriptor)
    {
        Descriptor = descriptor;
        Input = new ObservableCollection<IConnectorViewModel>();
        Output = new ObservableCollection<IConnectorViewModel>();
    }

    public virtual string Name => Descriptor.Name;
    public virtual Type ParentType => Descriptor.ComponentType;
    public virtual Type Type => Descriptor.PropertyType;

    public bool IsCollection => Type != null && Type != typeof(string) && typeof(IEnumerable).IsAssignableFrom(Type);

    [JsonIgnore]
    public virtual Type? CollectionItemPropertyType => Type?.IsArray == true ? Type.GetElementType() : IsCollection ? Type.GenericTypeArguments().SingleOrDefault() : null;

    public bool IsObservableCollection => Type != null && typeof(INotifyCollectionChanged).IsAssignableFrom(Type);
    public virtual bool IsCollectionItemValueType => CollectionItemPropertyType != null && CollectionItemPropertyType.IsValueType;

    public bool IsFlagsEnum => Type?.IsFlagsEnum() == true;

    public bool IsValueType => Type?.IsValueType == true;

    public override bool IsReadOnly => Descriptor.IsReadOnly;

    public virtual bool Equals(MemberDescriptor? other) => this.Name.Equals(other?.Name) && this.Type == other.ParentType;

    public override int GetHashCode() => this.Name.GetHashCode();

    public override bool HasChildren { get; } = true;
    public Descriptor Descriptor { get; }

    public override object Data { get => Descriptor; set => throw new Exception("sdf2g3r"); }

    public virtual void Initialise(object? item = null)
    {
    }

    public abstract IEnumerable Items();

    public virtual void Finalise(object? item = null)
    {
    }

    public Task<object> AsyncClone()
    {
        //return Task.FromResult<object>(this with { });
        throw new NotImplementedException();
    }

    public override sealed string ToString()
    {
        return Name;
    }
}

public abstract class ValueMemberDescriptor(Descriptor Descriptor) : MemberDescriptor(Descriptor), IValueDescriptor
{
    public abstract object? Get();

    public abstract void Set(object? value);
}

public abstract class ChildlessMemberDescriptor(Descriptor Descriptor) : MemberDescriptor(Descriptor)
{
    public override IEnumerable Items() => Array.Empty<object>();

    public override bool HasChildren => false;
}