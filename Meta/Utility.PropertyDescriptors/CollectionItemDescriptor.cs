using System.Collections;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
using Utility.Changes;
using Type = System.Type;
using Utility.Interfaces.NonGeneric;
using System.ComponentModel;

namespace Utility.PropertyDescriptors
{
    public record CollectionHeaderDescriptor : MemberDescriptor, ICollectionItemDescriptor, IEquatable
    {
        public CollectionHeaderDescriptor(Type propertyType, Type componentType) : base(propertyType)
        {
            this.ComponentType = componentType;
        }

        public override string? Name => "Header";

        public override bool IsReadOnly => true;

        public override string Category => "Collection";

        public override bool IsValueOrStringProperty => false;

        public override Type ComponentType { get; }

        public int Index => 0;

        public bool Equals(IEquatable? other)
        {
            if (other is CollectionHeaderDescriptor collectionHeaderDescriptor)
                return this.Type.Equals(collectionHeaderDescriptor.Type);
            return false;
        }

        public override object GetValue()
        {
            throw new NotImplementedException();
        }

        public override void SetValue(object value)
        {
            throw new NotImplementedException();
        }

        public virtual IObservable<Change<IMemberDescriptor>> GetChildren()
        {
            if (Type.GetConstructor(System.Type.EmptyTypes) == null || IsValueOrStringProperty)
                return Observable.Empty<Change<IMemberDescriptor>>();
          
            return Observable.Create<Change<IMemberDescriptor>>(observer =>
            {
                foreach(System.ComponentModel.PropertyDescriptor x in TypeDescriptor.GetProperties(Type))
                {
                    observer.OnNext(new(new HeaderDescriptor(x),Changes.Type.Add));
                }
                return Disposable.Empty;
            });
        }
    }

    public record HeaderDescriptor : MemberDescriptor
    {
        public HeaderDescriptor(System.ComponentModel.PropertyDescriptor propertyDescriptor) : base(propertyDescriptor.PropertyType)
        {
            ComponentType = propertyDescriptor.ComponentType;
        }

        public override string? Name => Type.Name;

        public override bool IsReadOnly => true;

        public override string Category => "Header";

        public override bool IsValueOrStringProperty => Type.IsValueOrStringProperty();

        public override Type ComponentType { get; }

        public override object GetValue()
        {
            throw new NotImplementedException();
        }

        public override void SetValue(object value)
        {
            throw new NotImplementedException();
        }
    }

    public partial record CollectionItemDescriptor : PropertyDescriptor, ICollectionItemDescriptor, IEquatable
    {
        private readonly Type componentType;

        public CollectionItemDescriptor(object item, int index, Type componentType) : base(new RootDescriptor(item))
        {
            Item = item;
            
            if (index == 0)
            {
                throw new Exception("Index 0 reserved for header!");
            }

            Index = index;
            this.componentType = componentType;
        }

        public object Item { get; set; }

        public int Index { get; }

        public override bool IsReadOnly => false;

        public override string? Name => Type.Name;

        public override string Category => "Item";

        public override Type ComponentType => componentType;

        public override bool IsValueOrStringProperty => Type.IsValueOrStringProperty();

        public override object GetValue()
        {
            return Item;
        }

        public override void SetValue(object? value)
        {
            if (Item is IList collection)
            {
                collection[Index] = value;
                Item = value;
                return;
            }
            throw new NotImplementedException();
        }


        public static int ToIndex(string name) => int.Parse(MyRegex().Matches(name).First().Groups[1].Value);
        public static string FromIndex(string name, int index) => name + $" [{index}]";

        public bool Equals(IEquatable? other)
        {
            if (other is CollectionItemDescriptor collectionItemDescriptor)
                return Item.Equals(collectionItemDescriptor.Item) && Index.Equals(collectionItemDescriptor.Index);
            return false;
        }

        [GeneratedRegex("\\[(\\d*)\\]")]
        private static partial Regex MyRegex();
    }
}


