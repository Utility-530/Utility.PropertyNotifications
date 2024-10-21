
namespace Utility.Descriptors
{
    internal record ParameterDescriptor(ParameterInfo ParameterInfo, Dictionary<int, object?> Component) : ValueMemberDescriptor(ParameterInfo.ParameterType)
    {
        public override Type ParentType => typeof(Dictionary<string, object?>);

        public override string? Name => ParameterInfo.Name ?? ParameterInfo.Position.ToString();

        public override bool IsReadOnly => false;

        public override IObservable<object> Children => Observable.Empty<object>();


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

