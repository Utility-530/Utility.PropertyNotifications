using System.Collections;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using Utility.Collections;
using Utility.Trees;
using Utility.Trees.Abstractions;

namespace Utility.Nodes
{
    public abstract class Node : Tree
    {
        private bool isRefreshing;
        bool flag;
        protected Collection items = new();

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
                return items;
            }
        }


        public virtual async Task<bool> RefreshChildrenAsync()
        {

            if (await HasMoreChildren() == false)
                return false;

            items.Clear();
            GetChildren()
                .Subscribe(async a =>
                {
                    var node =await ToNode(a);
                    items.Add(node);
                },
                e =>
                {
                },
                () =>
                {
                    //_isRefreshing = false;
                    items.Complete();
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