using System.Reactive.Linq;
using Utility.Collections;
using Utility.Models;
using Utility.PropertyDescriptors;
using Utility.PropertyNotifications;
using Utility.Trees.Abstractions;

namespace Utility.Nodes.Reflections
{
    public class ReflectionNode : Node
    {
        IMemberDescriptor data;

        public ReflectionNode(IMemberDescriptor propertyData)
        {

            this.data = propertyData;
        }

        public ReflectionNode()
        {
        }

        public override object Data
        {
            get
            {
                return data;
            }
            set => data = value as IMemberDescriptor;
        }

        protected override async Task<bool> RefreshChildrenAsync()
        {
            if (await HasMoreChildren() == false)
                return true;
            //if (data.GetValue() is not { } inst)
            //    return true;

            items.Clear();
            if (this.Key is Key { Guid: { } guid })
            {
                data
                   .GetChildren()
                   .Subscribe(async a =>
                   {
                       switch (a.Type)
                       {
                           case Changes.Type.Add:
                               {
                                   if (a.Value is not { } desc)
                                   {
                                       throw new Exception(" wrwe334 33");
                                   }
                                   else if (desc.Type == data.Type)
                                   { 
                                       var node = await ToNode(desc);
                                       items.Add(node);
                                   }
                                   else
                                   {
                                       //var conversion = ObjectConverter.ToValue(inst, desc);
                                       var node = await ToNode(desc);
                                       items.Add(node);
                                   }
                                   break;
                               }
                           case Changes.Type.Remove:
                               {

                                   var _guid = await GuidRepository.Instance.Find(guid, a.Value.Name);
                                   var key = new Key(_guid, a.Value.Name, a.Value.Type);
                                   items.RemoveOne(a => (a as IReadOnlyTree)?.Key.Equals(key) == true);
                                   break;
                               }
                           case Changes.Type.Reset:
                               {
                                   items.Clear();
                                   break;
                               }
                       }
                   },
                e =>
                {
                },
                () =>
                {
                    items.Complete();
                });

            }
            else
                throw new Exception("DFs 33312");
            return true;
        }


        public override IObservable<object?> GetChildren()
        {
            return data.GetChildren().Cast<object?>();
        }


        public override async Task<bool> HasMoreChildren()
        {
            return data.IsValueOrStringProperty == false && await base.HasMoreChildren();
        }

        public override string ToString()
        {
            return data?.Name;
        }


        public override async Task<IReadOnlyTree> ToNode(object value)
        {
            if (value is IMemberDescriptor { Name: string name } && this.Key is Key { Guid: { } guid })
            {

                var _guid = await GuidRepository.Instance.Find(guid, name);
                var key = new Key(_guid, name, null);
                if (value is MethodsDescriptor { } methodsData)
                {
                    return new ReflectionNode(methodsData) { Key = key, Parent = this };
                }
                else if (value is MethodDescriptor methodData)
                {
                    return new ReflectionNode(methodData) { Key = key, Parent = this };
                }
                else if (value is PropertiesDescriptor { } _propertyData)
                {
                    var node = new ReflectionNode(_propertyData) { Key = key, Parent = this };
                    return node;
                }
                else if (value is ParameterDescriptor { } parameterDescriptor)
                {
                    var node = new ReflectionNode(parameterDescriptor) { Key = key, Parent = this };
                    return node;
                }
                else if (value is CollectionItemDescriptor { } ciDescriptor)
                {
                    var node = new ReflectionNode(ciDescriptor) { Key = key, Parent = this };
                    return node;
                }
                else if (value is PropertyDescriptor { } propertyData)
                {
                    var node = new ReflectionNode(propertyData) { Key = key, Parent = this };
                    ValueRepository.Instance.Register(_guid, propertyData as INotifyPropertyCalled);
                    ValueRepository.Instance.Register(_guid, propertyData as INotifyPropertyReceived);
                    return node;
                }
                else
                {
                    throw new Exception("34422 2!pod");
                }
            }
            else
            {
                throw new Exception("32 2!pod");
            }
        }


    }
}
