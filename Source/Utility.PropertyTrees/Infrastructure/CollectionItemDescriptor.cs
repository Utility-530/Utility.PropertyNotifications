using System.ComponentModel;

namespace Utility.PropertyTrees.Infrastructure
{
    public class CollectionItemDescriptor : PropertyDescriptor
    {
        public CollectionItemDescriptor(object item, int index) : base(index.ToString(), null)
        {
            Item = item;
            Index = index;
        }

        public object Item { get; }

        public int Index { get; }

        public override System.Type ComponentType => throw new NotImplementedException();

        public override bool IsReadOnly => throw new NotImplementedException();

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
    }
}


