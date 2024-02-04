using Utility.Observables.Generic;
using Utility.Objects;
using Utility.Reactive.Helpers;
using Utility.Helpers;
using Utility.PropertyDescriptors;
using Utility.Trees.Abstractions;
using Utility.Models;
using Utility.Nodes.Reflections;
using System.Reactive.Linq;
using Utility.Collections;

namespace Utility.Nodes
{
    public class PropertiesNode : ReflectionNode
    {
        protected PropertyData data;


        public PropertiesNode(PropertyData propertyData)
        {
            if (propertyData.Descriptor == null)
            {
            }
            this.data = propertyData;
        }

        public PropertiesNode()
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

        protected override async Task<bool> RefreshChildrenAsync()
        {
            if (await HasMoreChildren() == false)
                return true;
            if (data.Descriptor.GetValue(data.Instance) is not { }  inst)
                return true;
 
            items.Clear();

            ChildPropertyExplorer
                .Explore(inst, data.Descriptor)
                .Subscribe(async a =>
                {
                    switch (a.ChangeType)
                    {
                        case ChangeType.Add:
                            {
                                if (a.Descriptor is not { } desc)
                                {
                                    throw new Exception(" wrwe334 33");
                                }
                                else if (desc.PropertyType == data.Type)
                                {

                                }
                                else
                                {
                                    var conversion = ObjectConverter.ToValue(inst, desc);
                                    var node = await ToNode(conversion);
                                    items.Add(node);
                                }
                                break;
                            }
                        case ChangeType.Remove:
                            {
                                if (this.Key is Key { Guid: { } guid })
                                {
                                    var _guid = await GuidRepository.Instance.Find(guid, a.Descriptor.Name);
                                    var key = new Key(_guid, a.Descriptor.Name, a.Descriptor.PropertyType);
                                    items.RemoveOne(a => (a as IReadOnlyTree)?.Key.Equals(key) == true);
                                }
                                break;
                            }
                        case ChangeType.Reset:
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
            return true;
        }
       
        public override async Task<bool> HasMoreChildren()
        {
            return data.Descriptor.IsValueOrStringProperty() == false && await base.HasMoreChildren();
        }

        public override string ToString()
        {
            return data?.Descriptor.Name;
        }
    }
}

