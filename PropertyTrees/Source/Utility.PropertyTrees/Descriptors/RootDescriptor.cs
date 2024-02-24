using System.ComponentModel;

namespace Utility.Descriptors
{
    public class RootDescriptor : PropertyDescriptor
    {
        public RootDescriptor(object item) : base("root", null)
        {
            Item = item;
        }

        public object Item { get; }

        public override Type ComponentType => null;

        public override bool IsReadOnly => true;

        public override Type PropertyType => Item.GetType();


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


