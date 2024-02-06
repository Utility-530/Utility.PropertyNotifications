using System.Collections;
using System.Text.RegularExpressions;
using Utility.Interfaces.NonGeneric;

namespace Utility.PropertyDescriptors
{
    public record CollectionItemDescriptor : PropertyDescriptor, ICollectionItemDescriptor, IEquatable
    {
        private readonly System.Type propertyType;
        private readonly System.Type componentType;

        public CollectionItemDescriptor(object item, int index, System.Type componentType) : base(new RootDescriptor(item))
        {
            Item = item;
            this.propertyType = item.GetType();
            this.Index = index;
            this.componentType = componentType;
        } 

        public object Item { get; set; }

        public int Index { get; }


        public override bool IsReadOnly => false;

        public override string? Name => propertyType.Name + $" [{Index}]";

        public override string Category => throw new NotImplementedException();

        public override System.Type ComponentType => componentType;

        public override bool IsValueOrStringProperty => propertyType.IsValueOrStringProperty();

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


        public static int ToIndex(string name) => int.Parse(Regex.Matches(name, @"\[(\d*)\]").First().Groups[1].Value);

        public bool Equals(IEquatable? other)
        {
            if (other is CollectionItemDescriptor collectionItemDescriptor)
                return Item.Equals(collectionItemDescriptor.Item) && Index.Equals(collectionItemDescriptor.Index);
            return false;
        }

    }
}


