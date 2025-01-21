using System.Reactive.Linq;
using System.Reactive.Subjects;
using Utility.Helpers.NonGeneric;
using Utility.Interfaces.NonGeneric;
using Utility.PropertyNotifications;
using Utility.Trees.Abstractions;

namespace Utility.Nodes
{

    public abstract class Node<T> : Utility.Nodes.Node, IExpand
    {
        protected ReplaySubject<Changes.Change<T>> changes = new();

        public Node(bool isExpanded = true)
        {
            this.IsExpanded = isExpanded;

            this.WithChangesTo(a => a.IsExpanded)
                .Subscribe(value =>
                {
                    if (value)
                    {
                        if (Data is IChildren children)
                            children.Children.Cast<Changes.Change<T>>().Subscribe(changes);
                    }
                    else
                    {
                        changes.OnNext(new Changes.Change<T>(default, Changes.Type.Reset));
                    }
                });
        }

        public virtual async Task<bool> RefreshChildrenAsync()
        {
            if (await HasMoreChildren() == false)
                return true;

            m_items.Clear();

            _ = GetChildren()
                .Cast<Changes.Change<T>>()
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
                                //else if (desc.Type == data?.Type)
                                //{
                                //    var node = await ToNode(desc);
                                //    m_items.Add(node);
                                //}
                                else
                                {
                                    //var conversion = ObjectConverter.ToValue(inst, desc);
                                    var node = await ToTree(desc);
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


        public IObservable<object?> GetChildren()
        {
            //if (Data is IChildren children)
            //    children.Children.Cast<Changes.Change<T>>().Subscribe(changes);
            return changes;
        }


        public override async Task<bool> HasMoreChildren()
        {
            return /*data?.Type.IsValueOrString() == false && */await base.HasMoreChildren();
        }
    }
}
