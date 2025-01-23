using System.ComponentModel;

namespace Utility.Meta
{
    public class RootDescriptor : PropertyDescriptor
    {
        private object component;
        public const string DefaultName = "root";

        public RootDescriptor(Type type, Type? parentType = null, string? name = null) : base(name ?? type.Name ?? DefaultName, null)
        {
            PropertyType = type;
            ComponentType = parentType;
        }

        public object Item => component;

        public override Type ComponentType { get; }

        public override bool IsReadOnly => true;

        public override Type PropertyType { get; }


        public override bool CanResetValue(object component)
        {
            throw new NotImplementedException();
        }

        public override object? GetValue(object? component)
        {
            if (component != null)
            {
                return this.component = component;
            }
            else
            {
                return this.component ??= Activator.CreateInstance(PropertyType);
            }
        }

        public override void ResetValue(object component)
        {
            throw new NotImplementedException();
        }

        public override void SetValue(object? component, object? value)
        {
            this.component = value;
        }

        public override bool ShouldSerializeValue(object component)
        {
            throw new NotImplementedException();
        }
    }
}