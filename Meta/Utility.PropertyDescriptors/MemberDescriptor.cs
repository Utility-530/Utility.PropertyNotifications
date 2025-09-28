using System.Collections.ObjectModel;
using System.Diagnostics;
using Utility.Interfaces;
using Utility.Interfaces.Exs.Diagrams;
using Utility.Interfaces.Generic;
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

    //[JsonIgnore]
    //public abstract IEnumerable Children { get; }

    public virtual bool Equals(MemberDescriptor? other) => this.Name.Equals(other?.Name) && this.Type == other.ParentType;

    public override int GetHashCode() => this.Name.GetHashCode();

    public override bool HasChildren { get; } = true;
    public Descriptor Descriptor { get; }

    public override object Data { get => Descriptor; set => throw new Exception("sdf2g3r"); }

    public virtual void Initialise(object? item = null)
    {
        //VisitChildren(this, a =>
        //{
        //    if (a is IDescriptor descriptor)
        //    {
        //        descriptor.Initialise();
        //    }
        //});
    }

    public abstract IEnumerable Items();

    public virtual void Finalise(object? item = null)
    {
        //VisitChildren(this, a =>
        //{
        //    if (a is IDescriptor descriptor)
        //    {
        //        descriptor.Finalise();
        //    }
        //});
    }
    //static void VisitChildren(IYieldItems tree, Action<object> action)
    //{
    //    tree.Items()
    //        .Cast<IDescriptor>()
    //        .ForEach(a =>
    //        {

    //            Trace.WriteLine(a.ParentType + " " + a.Type?.Name + " " + a.Name);
    //            action(a);

    //        });
    //}

    public Task<object> AsyncClone()
    {
        //return Task.FromResult<object>(this with { });
        throw new NotImplementedException();
    }

    public sealed override string ToString()
    {
        return Name;
    }
}

public abstract class ValueMemberDescriptor(Descriptor Descriptor) : MemberDescriptor(Descriptor), IValueDescriptor
{
    //[JsonIgnore]
    //public virtual object? Value
    //{
    //    get
    //    {
    //        var value = Get();
    //        RaisePropertyCalled(value);
    //        return value;
    //    }
    //    set
    //    {
    //        var _value = value;
    //        Set(value);
    //        RaisePropertyReceived(value, _value);
    //    }
    //}

    public abstract object? Get();

    public abstract void Set(object? value);

}

public abstract class ChildlessMemberDescriptor(Descriptor Descriptor) : MemberDescriptor(Descriptor)
{
    public override IEnumerable Items() => Array.Empty<object>();

    public override bool HasChildren => false;
}