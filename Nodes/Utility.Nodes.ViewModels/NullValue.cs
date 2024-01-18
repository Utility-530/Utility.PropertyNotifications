using System.ComponentModel;
using Utility.Interfaces.NonGeneric;

namespace Utility.Nodes
{
    public class NullValue : BasePropertyObject, IValue
    {

        public NullValue(PropertyDescriptor propertyDescriptor, object Instance) : base(propertyDescriptor, Instance)
        {
        }

        public override bool IsReadOnly => true;

        object? IValue.Value => null;
    }

}
