
namespace Utility.PropertyDescriptors
{
    public record RootDescriptor : PropertyDescriptor
    {
        public RootDescriptor(object item) : base(new RootPropertyDescriptor(item), item)
        {
        }
        public override bool IsValueOrStringProperty => false;
    }


    public class RootPropertyDescriptor : System.ComponentModel.PropertyDescriptor
    {
        public RootPropertyDescriptor(object item) : base(item?.GetType().Name ?? "root" , null)
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


