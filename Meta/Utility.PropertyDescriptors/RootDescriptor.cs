
namespace Utility.Descriptors
{
    public record RootDescriptor : PropertyDescriptor
    {
        public RootDescriptor(object item) : base(new RootPropertyDescriptor(item), item)
        {
        }
    }


    public class RootPropertyDescriptor : Descriptor
    {
        public RootPropertyDescriptor(object item) : this(item is Type type ? type : item.GetType())
        {
            if (item is not Type)
                Item = item;
        }

        public RootPropertyDescriptor(Type type) : base(type.Name ?? "root", null)
        {
            PropertyType = type;
        }


        public object Item { get; private set; }

        public override Type ComponentType => null;

        public override bool IsReadOnly => true;

        public override Type PropertyType { get; }


        public override bool CanResetValue(object component)
        {
            throw new NotImplementedException();
        }

        public override object? GetValue(object? component)
        {
            return Item ??= Activator.CreateInstance(PropertyType);
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


