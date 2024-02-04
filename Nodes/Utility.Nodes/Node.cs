using DynamicData;
using System.Collections;
using System.Reactive.Disposables;
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

        public abstract Task<object?> GetChildren();

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


        protected virtual async Task<bool> RefreshChildrenAsync()
        {

            if (await HasMoreChildren() == false)
                return false;

            var output = await GetChildren();
            if (output is IEnumerable enumerable)
            {
                items.Clear();
                ToNodes(enumerable)
                    .Subscribe(node => items.Add(node),
                    e =>
                    {

                    },
                    () =>
                    {
                        //_isRefreshing = false;
                        items.Complete();
                    });
            }

            return true;
        }


        public Exception Error { get; set; }

        protected virtual IObservable<IReadOnlyTree> ToNodes(IEnumerable collection)
        {
            return Observable.Create<IReadOnlyTree>(observer =>
            {
                CompositeDisposable disposable = new();
                foreach (var item in collection)
                {
                    ToNode(item)
                    .ToObservable()
                    .Subscribe(node =>
                    {
                        node.Parent = this;
                        observer.OnNext(node);
                    }).DisposeWith(disposable);
                }
                return disposable;
            });
        }

        public virtual Task<bool> HasMoreChildren()
        {
            return Task.FromResult(Data != null && flag == false);
        }
    }
}