using Utility.PropertyDescriptors;

namespace Utility.PropertyTrees
{
    public class RootProperty : PropertyBase
    {
        private RootDescriptor rootDescriptor;

        public RootProperty(Guid guid) : base(guid)
        {
 
        }

        public override string Name => PropertyType?.Name ?? "PropertyType not set";

        public override bool IsReadOnly => false;

        public override object Value { get; set; }

        public override bool HasChildren => true;


        public override System.ComponentModel.PropertyDescriptor Descriptor { get => rootDescriptor ??= new RootDescriptor(Data); set => throw new Exception(" sd333 32111"); }
    }
}