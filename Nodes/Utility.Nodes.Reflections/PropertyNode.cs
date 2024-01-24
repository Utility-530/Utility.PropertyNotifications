using Utility.Observables.Generic;
using Utility.Objects;
using Utility.Reactive.Helpers;
using Utility.Helpers;
using Utility.PropertyDescriptors;
using Utility.Trees.Abstractions;
using System;
using Utility.Models;
using System.ComponentModel;
using Utility.Nodes.Reflections;

namespace Utility.Nodes
{
    public class RootPropertyNode : PropertyNode
    {
        public RootPropertyNode(object instance) : base(new PropertyData(new RootDescriptor(instance), instance))
        {

        }
    }

    public class PropertyNode : Node
    {
        private PropertyData data;
        bool flag;

        public PropertyNode(PropertyData propertyData)
        {
            if (propertyData.Descriptor == null)
            {
            }
            this.data = propertyData;
        }
        
        
        
        public PropertyNode()
        {

        }

        public override object Data
        {
            get
            {
                return data;
            }
            set => data = value as PropertyData;
        }

        public override async Task<object?> GetChildren()
        {
            flag = true;
            var inst = data.Descriptor.GetValue(data.Instance);
            if (inst == null)
            {
                return Array.Empty<object>();
            }
            var children = (await ChildPropertyExplorer.Convert(inst, data.Descriptor));
            List<PropertyDescriptor> descriptors = new List<PropertyDescriptor>();
            foreach(var child in children)
            {
                if (child.Descriptor.PropertyType == data.Type)
                { 
                    
                }
                else
                {
                    descriptors.Add(child.Descriptor);
                }
            }

            if (data.Descriptor.IsValueOrStringProperty() == false && MethodExplorer.MethodInfos(data.Descriptor).Any())
                return descriptors.Select(a => ObjectConverter.ToValue(inst, a)).Append(new MethodsData(data.Descriptor, inst)).ToArray();
            return descriptors.Select(a => ObjectConverter.ToValue(inst, a)).ToArray();
        }

        public override string ToString()
        {
            return data?.Descriptor.Name;
        }

        public override async Task<IReadOnlyTree> ToNode(object value)
        {

            if (value is MethodsData { } methodsData)
            {
                if (this.Key is Key { Guid: { } guid })
                {
                    var _guid = await GuidRepository.Instance.Find(guid, "methods");
                    return new MethodsNode(methodsData) { Key = new Key(_guid, "methods", null) };
                }
                else
                    throw new Exception("f 32676 443opppp");
            }
            else if (value is PropertyData { Descriptor.Name: { } name } propertyData)
            {

                if (this.Key is Key { Guid: { } guid })
                {
                    var _guid = await GuidRepository.Instance.Find(guid, name);
                    return new PropertyNode(propertyData) { Key = new Key(_guid, name, propertyData.Type) };
                }
                else
                    throw new Exception("f 32443opppp");
            }

            else
                throw new Exception("34422 2!pod");
        }

        public override Task<bool> HasMoreChildren()
        {
            return Task.FromResult(data !=null && flag == false);
        }
    }
}

