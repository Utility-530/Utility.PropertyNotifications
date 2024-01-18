using System.ComponentModel;
using Utility.Interfaces.NonGeneric;

namespace Utility.Nodes
{
    public class DefaultValue : BasePropertyObject, IValue
    {
        public DefaultValue(PropertyDescriptor propertyDescriptor, object Instance) : base(propertyDescriptor, Instance)
        {

        }

        public object Value => Descriptor.GetValue(Instance);

        public override bool IsReadOnly=>true;
    }

}
