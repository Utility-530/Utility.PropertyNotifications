using System;
using System.Collections;
using System.Reactive.Threading.Tasks;
using Utility.Models;
using Utility.Objects;
using Utility.PropertyNotifications;
using Utility.Trees.Abstractions;

namespace Utility.Nodes.Reflections
{
    public class ReflectionNode : Node
    {     
        public override async Task<IReadOnlyTree> ToNode(object value)
        {

            if (value is MethodsData { } methodsData)
            {
                if (this.Key is Key { Guid: { } guid })
                {
                    var _guid = await GuidRepository.Instance.Find(guid, "methods");
                    return new MethodsNode(methodsData) { Key = new Key(_guid, "methods", null), Parent = this };
                }
                else
                    throw new Exception("f 32676 443opppp");
            }
            else if(value is MethodData { Info.Name: { } _name } methodData)
            {
                if (this.Key is Key { Guid: { } guid })
                {
                    var _guid = await GuidRepository.Instance.Find(guid, _name);
                    return new MethodNode(methodData) { Key = new Key(_guid, _name, null), Parent = this };
                }
                else
                    throw new Exception("f 32676 443opppp");
            }
            else if (value is PropertiesData { Descriptor.Name: { } __name } _propertyData)
            {
                if (this.Key is Key { Guid: { } guid })
                {
                    var _guid = await GuidRepository.Instance.Find(guid, __name);
                    var node = new PropertiesNode(_propertyData) { Key = new Key(_guid, __name, _propertyData.Type), Parent = this };
                    return node;
                }
                else
                    throw new Exception("f 32443opppp");
            }
            else if (value is PropertyData { Descriptor.Name: { } name } propertyData)
            {
                if (this.Key is Key { Guid: { } guid })
                {
                    var _guid = await GuidRepository.Instance.Find(guid, name);
                    var node = new PropertyNode(propertyData) { Key = new Key(_guid, name, propertyData.Type), Parent = this };
                    ValueRepository.Instance.Register(_guid, propertyData as INotifyPropertyCalled);
                    ValueRepository.Instance.Register(_guid, propertyData as INotifyPropertyReceived);
                    return node;
                }
                else
                    throw new Exception("f 32443opppp");
            }
            else
                throw new Exception("34422 2!pod");
        }


        public override Task<object?> GetChildren()
        {
            throw new NotImplementedException();
        }
    }
}
