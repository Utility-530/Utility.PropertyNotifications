using System.ComponentModel;

namespace Utility.PropertyTrees.Infrastructure
{
    public class RootDescriptor : PropertyDescriptor
    {
        public RootDescriptor(object item) : base("root", null)
        {
            Item = item;
        }

        public object Item { get; }

        public override System.Type ComponentType => null;

        public override bool IsReadOnly => true;

        public override System.Type PropertyType => Item.GetType();


        public override bool CanResetValue(object component)
        {
            throw new NotImplementedException();
        }

        public override object? GetValue(object? component)
        {
            throw new NotImplementedException();
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


