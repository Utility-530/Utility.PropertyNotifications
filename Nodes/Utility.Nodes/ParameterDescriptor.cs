using System.ComponentModel;
using System.Reflection;

namespace Utility.Nodes
{
    public class ParameterDescriptor : PropertyDescriptor
    {
        public ParameterDescriptor(ParameterInfo parameterInfo) : base(parameterInfo.Name ?? "p" + parameterInfo.Position.ToString(), null)
        {
            ParameterInfo = parameterInfo;

        }

        public ParameterInfo ParameterInfo { get; }

        public override Type ComponentType => typeof(Dictionary<string, object?>);

        public override bool IsReadOnly => false;

        public override Type PropertyType => ParameterInfo.ParameterType;

        public override bool CanResetValue(object component)
        {
            throw new NotImplementedException();
        }

        public override object? GetValue(object? component)
        {
            if (component is Dictionary<int, object?> dictionary && ParameterInfo.Position is int name)
                return dictionary[name] ?? ParameterInfo.DefaultValue;
            return ParameterInfo.DefaultValue;
        }

        public override void ResetValue(object component)
        {
            throw new NotImplementedException();
        }

        public override void SetValue(object? component, object? value)
        {
            if (component is Dictionary<int, object?> dictionary && ParameterInfo.Position is int name)
                dictionary[name] = value;
        }

        public override bool ShouldSerializeValue(object component)
        {
            throw new NotImplementedException();
        }
    }
}

