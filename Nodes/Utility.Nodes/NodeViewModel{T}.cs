using System.Collections;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using Utility.Changes;
using Utility.Helpers.NonGeneric;
using Utility.Interfaces.NonGeneric;
using Utility.PropertyNotifications;
using Utility.Trees.Abstractions;

namespace Utility.Nodes
{
    public abstract class NodeViewModel<T> : ViewModelTree, IExpand
    {
        protected ReplaySubject<Changes.Change<T>> changes = new();
        private bool isRefreshing;
        private bool flag;

        public NodeViewModel(bool isExpanded = true)
        {
            this.IsExpanded = isExpanded;

            this.WithChangesTo(a => a.IsExpanded)
                .CombineLatest(this.WithChangesTo(a => a.Data))
                .Subscribe(value =>
                {
                    if (value.First)
                    {
                        if (base.Data is IChanges children)
                            children.Changes.Cast<Change<T>>().Subscribe(changes);
                    }
                    else
                    {
                        changes.OnNext(new Changes.Change<T>(default, Utility.Changes.Type.Reset));
                    }
                });
        }

        public override IEnumerable Children
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

                .Subscribe<object>(async a =>
                {
                    if (a is Change<T> change)
                        switch (change.Type)
                        {
                            case Utility.Changes.Type.Add:
                                {
                                    if (change.Value is not { } desc)
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
                                        var node = await base.ToTree(desc);
                                        //await node.RefreshChildrenAsync();
                                        m_items.Add(node);
                                    }
                                    break;
                                }
                            case Utility.Changes.Type.Remove:
                                {
                                    m_items.RemoveOne(e => (e as IReadOnlyTree)?.Equals(change.Value) == true);
                                    break;
                                }
                            case Utility.Changes.Type.Reset:
                                {
                                    m_items.Clear();
                                    break;
                                }
                        }
                    else if (a is T t)
                    {
                        var node = await base.ToTree(t);
                        //await node.RefreshChildrenAsync();
                        m_items.Add(node);
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

        public virtual IObservable<object?> GetChildren()
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