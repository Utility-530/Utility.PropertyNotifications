using System.Reactive.Linq;
using Utility.Changes;
using Utility.Collections;
using Utility.Helpers;
using Utility.Helpers.NonGeneric;
using Utility.Interfaces.NonGeneric;
using Utility.Keys;
using Utility.Models;
using Utility.Trees.Abstractions;

namespace Utility.Nodes.Reflections
{
    public class ReflectionNode : Node
    {
        IDescriptor data;

        public ReflectionNode(IDescriptor propertyData)
        {
            this.data = propertyData;
            Key = new GuidKey(data.Guid);
        }

        public ReflectionNode()
        {
        }

        public override object Data
        {
            get => data;
            set
            {
                data = value as IDescriptor;
                Key = new GuidKey(data.Guid);
            }
        }

        public override async Task<bool> RefreshChildrenAsync()
        {
            if (await HasMoreChildren() == false)
                return true;

            m_items.Clear();

            _ = GetChildren()
                .Cast<Change<IDescriptor>>()
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
                                else if (desc.Type == data?.Type)
                                {
                                    var node = await ToNode(desc);
                                    m_items.Add(node);
                                }
                                else
                                {
                                    //var conversion = ObjectConverter.ToValue(inst, desc);
                                    ReflectionNode node = (ReflectionNode)await ToNode(desc);
                                    //await node.RefreshChildrenAsync();
                                    m_items.Add(node);
                                }
                                break;
                            }
                        case Changes.Type.Remove:
                            {
                                m_items.RemoveOne(e => (e as IReadOnlyTree)?.Data.Equals(a.Value) == true);
                                break;
                            }
                        case Changes.Type.Reset:
                            {
                                m_items.Clear();
                                break;
                            }
                    }
                },
            e =>
            {
            },
            () =>
            {
                //items.Complete();
            });

            return true;
        }


        public override IObservable<object?> GetChildren()
        {
            if (data is IChildren children)
                return children.Children;
            return Observable.Empty<object?>();
        }


        public override async Task<bool> HasMoreChildren()
        {
            return /*data?.Type.IsValueOrString() == false && */await base.HasMoreChildren();
        }

        public override string ToString()
        {
            return data?.Name;
        }


        public override async Task<IReadOnlyTree> ToNode(object value)
        {
            if (value is IDescriptor { } descriptor)
            {

                return new ReflectionNode(descriptor) { Parent = this };
            }
            else
            {
                throw new Exception("32 2!pod");
            }
        }
    }
}
