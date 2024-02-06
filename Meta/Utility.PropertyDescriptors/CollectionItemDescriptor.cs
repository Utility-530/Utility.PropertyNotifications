using System.Collections;
using System.Text.RegularExpressions;
using Utility.Interfaces.NonGeneric;

namespace Utility.PropertyDescriptors
{
    public partial record CollectionItemDescriptor : PropertyDescriptor, ICollectionItemDescriptor, IEquatable
    {
        private readonly Type propertyType;
        private readonly Type componentType;

        public CollectionItemDescriptor(object item, int index, Type componentType) : base(new RootDescriptor(item))
        {
            Item = item;
            propertyType = item.GetType();
            Index = index;
            this.componentType = componentType;
        }

        public object Item { get; set; }

        public int Index { get; }

        public override bool IsReadOnly => false;

        public override string? Name => propertyType.Name;

        public override string Category => throw new NotImplementedException();

        public override Type ComponentType => componentType;

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


