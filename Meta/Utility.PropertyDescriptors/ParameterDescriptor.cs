using System.ComponentModel;
using System.Reactive.Linq;
using System.Reflection;
using Utility.Interfaces.NonGeneric;

namespace Utility.PropertyDescriptors
{
    public record ParameterDescriptor(ParameterInfo ParameterInfo, Dictionary<int, object?> Component) : BaseDescriptor(ParameterInfo.ParameterType), IIsReadOnly, IType, IValue
    {
        public override Type ComponentType => typeof(Dictionary<string, object?>);

        public override bool IsReadOnly => false;


        public override string? Name => ParameterInfo.Name ?? ParameterInfo.Position.ToString();

        public override string Category => throw new NotImplementedException();

        public override bool IsValueOrStringProperty => Type.IsValueOrStringProperty();

        public object Value { get => GetValue(); set => SetValue(value); }

        //public override IObservable<Changes.Change<IMemberDescriptor>> GetChildren()
        //{
        //    return Observable.Empty<Changes.Change<IMemberDescriptor>>();
        //}

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

