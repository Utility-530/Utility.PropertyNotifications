using System.Collections;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using Utility.Collections;
using Utility.Interfaces.NonGeneric;
using Utility.Trees;
using Utility.Trees.Abstractions;

namespace Utility.Nodes
{
    public abstract class Node : ObservableTree
    {
        private bool isRefreshing;
        bool flag;
        //protected Collection items = new();

        public abstract IObservable<object?> GetChildren();

        public abstract Task<IReadOnlyTree> ToNode(object value);

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
                return false;

            m_items.Clear();
            GetChildren()
                .Subscribe(async a =>
                {
                    var node =await ToNode(a);
                    m_items.Add(node);
                },
                e =>
                {
                },
                () =>
                {
                    //_isRefreshing = false;
                    if(m_items is IComplete complete)
                        complete.Complete();
                });
            return true;
        }


        public Exception Error { get; set; }

        public virtual Task<bool> HasMoreChildren()
        {
            return Task.FromResult(Data != null && flag == false);
        }
    }
}