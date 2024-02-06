using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Utility.Changes;
using Utility.Helpers;
using Utility.Interfaces.Generic;
using Utility.Nodes.Reflections;
using Utility.PropertyNotifications;


namespace Utility.PropertyDescriptors
{
    public abstract record PropertyDescriptor<T>(System.ComponentModel.PropertyDescriptor Descriptor, object Instance) : PropertyDescriptor(Descriptor, Instance), IValue<T>, IRaisePropertyChanged
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
        object Interfaces.NonGeneric.IValue.Value => Value;

        public void RaisePropertyChanged(object value, string? propertyName = null)
        {
            if (Descriptor.IsReadOnly == true)
                return;
            Descriptor.SetValue(Instance, value);
            base.RaisePropertyChanged(nameof(Value));
        }
    }

    public abstract record NullablePropertyDescriptor<T>(System.ComponentModel.PropertyDescriptor Descriptor, object Instance) : PropertyDescriptor(Descriptor, Instance), IValue<T?>
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
        object Interfaces.NonGeneric.IValue.Value => Value;
    }

    public abstract record PropertyDescriptor(System.ComponentModel.PropertyDescriptor Descriptor, object Instance) : MemberDescriptor(Descriptor.PropertyType)
    {

        public override System.IObservable<Change<IMemberDescriptor>> GetChildren()
        {
            if (Descriptor.GetValue(Instance) is not { } inst)
                return Observable.Empty<Change<IMemberDescriptor>>();

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
        }

        public override System.Type ComponentType => Descriptor.ComponentType;

        public override bool IsReadOnly => Descriptor.IsReadOnly;

        public override bool IsValueOrStringProperty => Descriptor.IsValueOrStringProperty();

        public override string? Name => Descriptor.Name;

        public override string Category => string.IsNullOrWhiteSpace(Descriptor.Category) ||
                Descriptor.Category.EqualsIgnoreCase(CategoryAttribute.Default.Category)
                    ? null
                    : Descriptor.Category;

        public override object? GetValue()
        {
            return Descriptor.GetValue(Instance);
        }

        public override void SetValue(object value)
        {
            Descriptor.SetValue(value, Instance);
        }

        // collection
        public virtual int CollectionCount => Instance is IEnumerable enumerable ? enumerable.Cast<object>().Count() : 0;


    }


    public abstract record MemberDescriptor(System.Type Type) : NotifyProperty, IMemberDescriptor
    {
        public Guid Guid { get; set; }

        public abstract string? Name { get; }

        public abstract bool IsReadOnly { get; }

        public abstract string Category { get; }

        public abstract bool IsValueOrStringProperty { get; }

        public abstract System.Type ComponentType { get; }

        //public abstract System.IObservable<Change<IMemberDescriptor>> GetChildren();

        public virtual System.IObservable<Change<IMemberDescriptor>> GetChildren()
        {
            if (Type.GetConstructor(System.Type.EmptyTypes) == null || IsValueOrStringProperty)
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

        public virtual bool Equals(MemberDescriptor? other)
        {
            return this.Guid.Equals(other?.Guid);
        }

        public override int GetHashCode()
        {
            return this.Guid.GetHashCode();
        }
    }
}


