using System.Collections;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using Utility.Helpers.NonGeneric;
using Utility.Interfaces.NonGeneric;
using Utility.PropertyNotifications;
using Utility.Trees.Abstractions;

namespace Utility.Nodes
{

    public abstract class Node<T> : ViewModelTree, IExpand
    {
        protected ReplaySubject<Changes.Change<T>> changes = new();
        private bool isRefreshing;
        bool flag;

        public Node(bool isExpanded = true)
        {
            this.IsExpanded = isExpanded;

            this.WithChangesTo(a => a.IsExpanded)
                .CombineLatest(this.WithChangesTo(a => a.Data).Where(a => a != null))
                .Subscribe(value =>
                {
                    if (value.First)
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



        public override IEnumerable Items
        {
            get
            {
                try
                {
                    if (!isRefreshing)
                    {
                        isRefreshing = true;
                        _ = RefreshChildrenAsync()
                            .ToObservable()
                            .Subscribe(a =>
                            {
                                flag = a;
                                isRefreshing = false;
                            });
                        flag = true;
                    }
                }
                catch (Exception ex)
                {
                    isRefreshing = false;
                    Error = ex;
                }
                finally
                {
                }
                return m_items;
            }
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
            return changes;
        }

        public override Task<bool> HasMoreChildren()
        {
            return Task.FromResult(Data != null && flag == false);
        }


        public Exception Error { get; set; }


    }
}

