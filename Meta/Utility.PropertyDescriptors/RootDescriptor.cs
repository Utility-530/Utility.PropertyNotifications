
namespace Utility.Descriptors
{
    public record RootDescriptor : PropertyDescriptor
    {
        public RootDescriptor(object item, string? name = default) : base(new RootPropertyDescriptor(item, name), item)
        {
        }
    }


    public class RootPropertyDescriptor : Descriptor
    {
        public RootPropertyDescriptor(object item, string? name = null) : this(item is Type type ? type : item.GetType(), name)
        {
            if (item is not Type)
                Item = item;
            Name = name;
        }

        public RootPropertyDescriptor(Type type, string? name = null) : base(type.Name ?? "root", null)
        {
            PropertyType = type;
            Name = name;
        }

        public override string Name { get;  } 
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


