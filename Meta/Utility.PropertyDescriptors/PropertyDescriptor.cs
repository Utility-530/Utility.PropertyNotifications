
using Utility.Interfaces.Generic;

namespace Utility.Descriptors;

public abstract record PropertyDescriptor<T>(Descriptor Descriptor, object Instance) : PropertyDescriptor(Descriptor, Instance), IValue<T>, IRaisePropertyChanged
{
    public T Value
    {
        get
        {
            var value = (T)Descriptor.GetValue(Instance);
            this.RaisePropertyCalled(value);
            return value;
        }
        set
        {
            if (Descriptor.IsReadOnly == true)
                return;
            Descriptor.SetValue(Instance, value);
            this.RaisePropertyReceived(value);
        }
    }
    object IValue.Value => Value;

    public void RaisePropertyChanged(object value, string? propertyName = null)
    {
        if (Descriptor.IsReadOnly == true)
            return;
        Descriptor.SetValue(Instance, value);
        base.RaisePropertyChanged(nameof(Value));
    }
}

public abstract record NullablePropertyDescriptor<T>(Descriptor Descriptor, object Instance) : PropertyDescriptor(Descriptor, Instance), IValue<T?>
{
    public T? Value
    {
        get
        {
            var value = GetValue() is T t ? t : default; ;
            this.RaisePropertyCalled(value);
            return value;
        }

        set
        {
            SetValue(value);
            this.RaisePropertyReceived(value);
        }
    }
    object IValue.Value => Value;
}

public abstract record PropertyDescriptor(Descriptor Descriptor, object Instance) : MemberDescriptor(Descriptor.PropertyType), IIsReadOnly
{
    public override System.IObservable<Change<IMemberDescriptor>> GetChildren()
    {
        if (Descriptor.GetValue(Instance) is { } inst)
            return Observable.Create<Change<IMemberDescriptor>>(async observer =>
            {
                var propertiesDescriptor = new PropertiesDescriptor(Descriptor, inst);
                var pguid = await GuidRepository.Instance.Find(this.Guid, propertiesDescriptor.Name);
                observer.OnNext(new(propertiesDescriptor with { Guid = pguid }, Changes.Type.Add));

                var collectionDescriptor = new CollectionDescriptor(Descriptor, inst);
                var cguid = await GuidRepository.Instance.Find(this.Guid, collectionDescriptor.Name);
                observer.OnNext(new(collectionDescriptor with { Guid = cguid }, Changes.Type.Add));

                var methodsDescriptor = new MethodsDescriptor(Descriptor, inst);
                var mguid = await GuidRepository.Instance.Find(this.Guid, methodsDescriptor.Name);
                observer.OnNext(new(methodsDescriptor with { Guid = mguid }, Changes.Type.Add));
                return Disposable.Empty;
            });
        else
            return Observable.Empty<Change<IMemberDescriptor>>();
    }

    public override Type ParentType => Descriptor.ComponentType;

    public bool IsReadOnly => Descriptor.IsReadOnly;

    public override string? Name => Descriptor.Name;

    public override object? GetValue()
    {
        return Descriptor.GetValue(Instance);
    }

    public override void SetValue(object value)
    {
        Descriptor.SetValue(Instance, value);
    }

}


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


