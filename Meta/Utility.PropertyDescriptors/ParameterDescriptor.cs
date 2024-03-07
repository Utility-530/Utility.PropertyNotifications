
namespace Utility.Descriptors
{
    public record ParameterDescriptor(ParameterInfo ParameterInfo, Dictionary<int, object?> Component) : MemberDescriptor(ParameterInfo.ParameterType)
    {
        public override Type ParentType => typeof(Dictionary<string, object?>);

        public override string? Name => ParameterInfo.Name ?? ParameterInfo.Position.ToString();

        public object Value { get => GetValue(); set => SetValue(value); }

        public override IObservable<Change<IMemberDescriptor>> GetChildren()
        {
            return Observable.Empty<Change<IMemberDescriptor>>();   
        }

        public override object? GetValue()
        {
            if (Component is Dictionary<int, object?> dictionary && ParameterInfo.Position is int name)
                return dictionary[name] ?? ParameterInfo.DefaultValue;
            return ParameterInfo.DefaultValue;
        }

        public override void SetValue(object? value)
        {
            if (Component is Dictionary<int, object?> dictionary && ParameterInfo.Position is int name)
                dictionary[name] = value;
        }

    }
}

