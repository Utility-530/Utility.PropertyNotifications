using Utility.Helpers;
using Utility.Nodes.Reflections;
using Utility.Objects;

namespace Utility.Nodes
{

    public class MethodNode : ReflectionNode
    {
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
            var x = a.HasDefaultValue ? a.DefaultValue : AlternateValue(a);
            return x;
            static object? AlternateValue(System.Reflection.ParameterInfo a)
            {
                if (a.ParameterType.IsValueType || a.ParameterType.GetConstructor(Type.EmptyTypes) != null)
                    return Activator.CreateInstance(a.ParameterType);
                return null;
            }
        }

        public override object Data => new CommandValue(data.Info, data.Instance, dictionary);

        public override async Task<object?> GetChildren()
        {
            return await Task.FromResult(data.Info.ParameterDescriptors().Where(a=> a.PropertyType!= (this.Parent.Data as PropertyData)?.Type).Select(a => ObjectConverter.ToValue(dictionary, a)));
        }

        public override string ToString()
        {
            return data.Info.Name;
        }

    }

}
