using Utility.Helpers;
using Utility.Interfaces.NonGeneric;
using Utility.Properties;

namespace Utility.Nodes
{

    public class MethodNode : Node
    {
        private bool flag;

        private MethodData data;
        private Dictionary<int, object?> dictionary;
        public MethodNode(MethodData propertyData)
        {
            this.data = propertyData;
            dictionary = data.Info
                            .GetParameters()
                            .ToDictionary(a => a.Position, a => GetValue(a));
        }

        private static object? GetValue(System.Reflection.ParameterInfo a)
        {
            var x =a.HasDefaultValue? a.DefaultValue : AlternateValue(a);
            return x;
            static object? AlternateValue(System.Reflection.ParameterInfo a)
            {
                if (a.ParameterType.IsValueType)
                    return Activator.CreateInstance(a.ParameterType);
                if (a.ParameterType.GetConstructor(Type.EmptyTypes) != null)
                    return Activator.CreateInstance(a.ParameterType);
                return null;
            }
        }

        public override object Data => new CommandValue(data.Info, data.Instance, dictionary);

        public override IEquatable Key => null;

        public override async Task<object?> GetChildren()
        {
            flag = true;
            return await Task.FromResult(data.Info.ParameterDescriptors().Select(a => new ParameterData(dictionary, a)));
        }

        public override string ToString()
        {
            return data.Info.Name;
        }

        public override Node ToNode(object value)
        {
            return new ParameterNode(value as ParameterData);
        }

        public override Task<bool> HasMoreChildren()
        {
            return Task.FromResult(flag == false);
        }
    }

}
