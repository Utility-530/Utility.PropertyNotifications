using System.Collections.ObjectModel;
using System.ComponentModel.Design.Serialization;
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

public abstract class MemberDescriptor : NodeViewModel, IPropertyDescriptor, IInstance, IDescriptor, IIsReadOnly, ICollectionDetailsDescriptor, IAsyncClone, IHasChildren
{
    public MemberDescriptor(Descriptor descriptor)
    {
        Descriptor = descriptor;
        Inputs = new ObservableCollection<IConnectorViewModel>();
        Outputs = new ObservableCollection<IConnectorViewModel>();
        isProliferable = false;
        Name = Descriptor.Name; 
    }
    public MemberDescriptor(Descriptor descriptor, object instance)
    {
        Instance = instance;
    }

    public object Instance { get; } 

    public virtual Type ParentType => Descriptor.ComponentType;
    public override Type Type => Descriptor.PropertyType;

    public bool IsCollection => Type != null && Type != typeof(string) && typeof(IEnumerable).IsAssignableFrom(Type);

    [JsonIgnore]
    public virtual Type? CollectionItemPropertyType => Type?.IsArray == true ? Type.GetElementType() : IsCollection ? Type.GenericTypeArguments().SingleOrDefault() : null;

    public bool IsObservableCollection => Type != null && typeof(INotifyCollectionChanged).IsAssignableFrom(Type);
    public virtual bool IsCollectionItemValueType => CollectionItemPropertyType != null && CollectionItemPropertyType.IsValueType;

    public bool IsFlagsEnum => Type?.IsFlagsEnum() == true;

    public bool IsValueType => Type?.IsValueType == true;

    public override bool IsReadOnly => Descriptor.IsReadOnly;

    public virtual bool Equals(MemberDescriptor? other) => this.Key != null ? this.Key.Equals(other?.Key) : this.Name.Equals(other?.Name) && this.Type == other.ParentType;

    public override int GetHashCode() => this.Key?.GetHashCode() ?? this.Name.GetHashCode();

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

    public override sealed string ToString()
    {
        return Name;
    }
}

public abstract class ValueMemberDescriptor : MemberDescriptor, IValueDescriptor
{
    public ValueMemberDescriptor(Descriptor Descriptor):base(Descriptor)
    {
        Name = Descriptor.PropertyType.Name;
    }
    public ValueMemberDescriptor(Descriptor descriptor, object instance):base(descriptor, instance)
    {
        Name = descriptor.PropertyType.Name;
    }
    public abstract object? Get();

    public abstract void Set(object? value);
}

public abstract class ChildlessMemberDescriptor(Descriptor Descriptor) : MemberDescriptor(Descriptor)
{
    public override IEnumerable Items() => Array.Empty<object>();

    public override bool HasChildren => false;
}