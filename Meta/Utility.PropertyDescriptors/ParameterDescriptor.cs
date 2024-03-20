
namespace Utility.Descriptors
{
    public record ParameterDescriptor(ParameterInfo ParameterInfo, Dictionary<int, object?> Component) : MemberDescriptor(ParameterInfo.ParameterType)
    {
        public override Type ParentType => typeof(Dictionary<string, object?>);

        public override string? Name => ParameterInfo.Name ?? ParameterInfo.Position.ToString();

        public object Value { get => Get(); set => Set(value); }

        public override bool IsReadOnly => false;


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

