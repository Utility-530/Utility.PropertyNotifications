using System.Reactive.Disposables;
using System.Reactive.Linq;
using Utility.Trees.Abstractions;

namespace Utility.Trees.Extensions.Async
{
    public static class Generic
    {
        ///// <summary> Converts given collection to tree. </summary>
        ///// <typeparam name="T">Custom data type to associate with tree ITree.</typeparam>
        ///// <param name="items">The collection items.</param>
        ///// <param name="parentSelector">Expression to select parent.</param>
        public static IObservable<ITree<T>> ToTree<T, K>(this IObservable<T> collection, Func<T, K> id_selector, Func<T, K> parent_id_selector, K? root_id = default, Func<T, ITree<T>>? func = null)
        {
            return Observable.Create<ITree<T>>(observer =>
            {
                CompositeDisposable disposables = new();
                var dis = collection
                            .Where(c => EqualityComparer<K>.Default.Equals(parent_id_selector(c), root_id))
                            .Subscribe(a =>
                            {
                                var tree = func?.Invoke(a) ?? new Tree<T>(a);
                                observer.OnNext(tree);
                                var _dis = ToTree(collection, id_selector, parent_id_selector, id_selector(a))
                                     .Subscribe(a =>
                                     {
                                         tree.Add(a);
                                     });
                                disposables.Add(_dis);
                            });
                disposables.Add(dis);
                return disposables;
            });
        }
    }
}