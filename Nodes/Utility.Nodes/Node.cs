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
        private bool _isRefreshing;
        protected Collection items = new();

        public abstract Task<object?> GetChildren();

        public abstract Task<bool> HasMoreChildren();

        public abstract Task<IReadOnlyTree> ToNode(object value);

        public override IEnumerable Items
        {
            get
            {
                _ = RefreshChildrenAsync();
                return items;
            }
        }


        protected virtual async Task<bool> RefreshChildrenAsync()
        {
            if (_isRefreshing)
                return false;

            if (await HasMoreChildren() == false)
                return false;

            _isRefreshing = true;

            try
            {

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
                            _isRefreshing = false;
                            items.Complete();
                        });
                }

                return true;
            }
            catch (Exception ex)
            {
                Error = ex;
                return false;
            }
            finally
            {
                _isRefreshing = false;
            }

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
    }
}