
using Splat;
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

public record PropertyDescriptor(Descriptor Descriptor, object Instance) : MemberDescriptor(Descriptor.PropertyType), IIsReadOnly
{
    public override System.IObservable<Change<IMemberDescriptor>> GetChildren()
    {
        if (Descriptor.GetValue(Instance) is var inst)
        {
            if(inst is null)
            {
                return Observable.Empty<Change<IMemberDescriptor>>();
            }
            if (inst is Type type)
            {
                return Observable.Empty<Change<IMemberDescriptor>>();
            }
            return Observable.Create<Change<IMemberDescriptor>>(async observer =>
            {

                var repo = Locator.Current.GetService<ITreeRepository>();
                var propertiesDescriptor = new PropertiesDescriptor(Descriptor, inst);
                var pguid = await repo.Find(this.Guid, propertiesDescriptor.Name);
                observer.OnNext(new(propertiesDescriptor with { Guid = pguid }, Changes.Type.Add));

                var collectionDescriptor = new CollectionDescriptor(Descriptor, inst);
                var cguid = await repo.Find(this.Guid, collectionDescriptor.Name);
                observer.OnNext(new(collectionDescriptor with { Guid = cguid }, Changes.Type.Add));

                //var methodsDescriptor = new MethodsDescriptor(Descriptor, inst);
                //var mguid = await repo.Find(this.Guid, methodsDescriptor.Name);
                //observer.OnNext(new(methodsDescriptor with { Guid = mguid }, Changes.Type.Add));
                return Disposable.Empty;
            });
        }
        return Observable.Empty<Change<IMemberDescriptor>>();
    }

    //public DateValue LastPersistence { get; set; }

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


