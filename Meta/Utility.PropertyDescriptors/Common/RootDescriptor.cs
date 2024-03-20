using Splat;
using System.ComponentModel;

namespace Utility.Descriptors
{
    public class RootDescriptor : Descriptor
    {
        private object component;

        public RootDescriptor(Type type, Type? parentType = null, string? name = null) : base(type.Name ?? "root", null)
        {
            PropertyType = type;
            ComponentType = parentType;
            Name = name;
        }

        public object Item => this.component;

        public override string Name { get; }

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