using System.ComponentModel;
using System.Text.RegularExpressions;

namespace Utility.PropertyTrees.Infrastructure
{
    public class CollectionItemDescriptor : PropertyDescriptor
    {
        public CollectionItemDescriptor(object item, int index, Type componentType) : base(item.GetType().Name + $" [{index}]", null)
        {
            Item = item;
            Index = index;
            this.ComponentType = componentType;
        }

        public object Item { get; }

        public int Index { get; }

        public override System.Type ComponentType { get; }

        public override bool IsReadOnly => false;

        public override System.Type PropertyType => Item.GetType();


        public override bool CanResetValue(object component)
        {
            throw new NotImplementedException();
        }

        public override object? GetValue(object? component)
        {
            return Item;
        }

        public override void ResetValue(object component)
        {
            throw new NotImplementedException();
        }

        public override void SetValue(object? component, object? value)
        {
            throw new NotImplementedException();
        }

        public override bool ShouldSerializeValue(object component)
        {
            throw new NotImplementedException();
        }

        public static int ToIndex(string name) => int.Parse(Regex.Matches(name, @"\[(\d*)\]").First().Groups[1].Value);
    }
}


