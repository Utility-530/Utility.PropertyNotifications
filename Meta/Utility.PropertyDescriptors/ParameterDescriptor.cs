
using Utility.Meta;

namespace Utility.PropertyDescriptors
{
    internal record ParameterDescriptor(ParameterInfo ParameterInfo, Dictionary<int, object?> Component) : ValueMemberDescriptor(new RootDescriptor(ParameterInfo.ParameterType, typeof(Dictionary<string, object?>), ParameterInfo.Name ?? ParameterInfo.Position.ToString()))
    {
        public override bool IsReadOnly => false;

        public override IEnumerable<object> Children => Array.Empty<object>();

        public override bool HasChildren => false;

        public override void Finalise(object? item = null)
        {
         
        }
        public override void Initialise(object? item = null)
        {
   
        }

        public override object? Get()
        {
            if (Component is Dictionary<int, object?> dictionary && ParameterInfo.Position is int name)
                return dictionary[name] ?? ParameterInfo.DefaultValue;
            return ParameterInfo.DefaultValue;
        }

        public override void Set(object? value)
        {
            if (Component is Dictionary<int, object?> dictionary && ParameterInfo.Position is int name)
            {
                dictionary[name] = value;
                //return true;
            }
            //return false;
        }
    }
}

