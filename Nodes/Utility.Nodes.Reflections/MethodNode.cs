using System;
using Utility.Helpers;
using Utility.Interfaces.NonGeneric;
using Utility.Models;
using Utility.Nodes.Reflections;
using Utility.Objects;
using Utility.Trees.Abstractions;

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
            flag = true;
            return await Task.FromResult(data.Info.ParameterDescriptors().Select(a => new ParameterData(a, dictionary)));
        }

        public override async Task<IReadOnlyTree> ToNode(object value)
        {
  
            if (value is ParameterData { Descriptor.Name: { } name } propertyData)
            {
                if (this.Key is Key { Guid: { } guid })
                {
                    var _guid = await GuidRepository.Instance.Find(guid, name);
                    return new ParameterNode(propertyData) { Key = new Key(_guid, name, propertyData.Type) };
                }
                else
                    throw new Exception("f 32443opppp");
            }
            throw new Exception("67676f 32443opppp");
        }

        public override Task<bool> HasMoreChildren()
        {
            return Task.FromResult(flag == false);
        }
        public override string ToString()
        {
            return data.Info.Name;
        }

    }

}
