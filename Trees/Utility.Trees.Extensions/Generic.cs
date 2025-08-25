using System.Collections;
using System.Collections.Specialized;
using System.Reactive.Linq;
using Utility.Helpers.NonGeneric;
using Utility.Interfaces.Generic;
using Utility.Interfaces.NonGeneric;
using Utility.Reactives;
using Utility.Trees.Abstractions;

namespace Utility.Trees.Extensions
{
    public static class Generic
    {

        public static void Foreach<T>(Func<T, IReadOnlyTree, int, T> action, IEnumerable collection, T parent, int level = 0)
        {
            foreach (IReadOnlyTree item in collection)
            {
                var newTtem = action(parent, item, level);
                Foreach(action, item.Items, newTtem, ++level);
            }
        }


        public static int[] Index<T>(T node, Func<T, T?> getParent, Func<T, T, int> getChildIndex)
        {
            var path = new List<int>();
            var current = node;

            while (getParent(current) is T parent)
            {
                int index = getChildIndex(parent, current);
                if (index < 0)
                    throw new ArgumentException("Child not found in parent");

                path.Add(index);
                current = parent;
            }
            path.Add(0);
            path.Reverse(); // Convert from root-to-leaf order
            return [.. path];
        }

        public static IEnumerable<ITree<T>> ToTree<T, K>(this IEnumerable<T> collection, Func<T, K> id_selector, Func<T, K> parent_id_selector, T root)
        {
            return ToTree<T, K, ITree<T>>(collection, id_selector, parent_id_selector, (a, r) => (ITree<T>)new Tree<T>(a) { Parent = r }, root, null);
        }

        public static IEnumerable<TTree> ToTree<T, K, TTree>(this IEnumerable<T> collection, Func<T, K> id_selector, Func<T, K> parent_id_selector, Func<T, TTree?, TTree> conversion, T root, TTree? rootTree = default)
        {
            foreach (var item in collection.Where(c => EqualityComparer<K>.Default.Equals(parent_id_selector(c), id_selector(root))))
            {
                var tree = conversion(item, rootTree);
                yield return tree;
                foreach (var x in ToTree(collection, id_selector, parent_id_selector, conversion, item, tree))
                    yield return x;
            }
        }

        public static IObservable<TTree> ToTree<X, T, K, TTree>(this X collection, Func<T, K> id_selector, Func<T, K> parent_id_selector, Func<T, TTree?, TTree> conversion, T root, TTree? rootTree = default)
            where X : INotifyCollectionChanged, IEnumerable<T>
        {
            return Observable.Create<TTree>(observer =>
            {
                return collection
                    .AndAdditions<T>()
                    .Where(c => EqualityComparer<K>.Default.Equals(parent_id_selector(c), id_selector(root)))
                    .Subscribe(item =>
                    {
                        Task.Run(() =>
                        {
                            var tree = conversion(item, rootTree);
                            observer.OnNext(tree);

                            ToTree(collection, id_selector, parent_id_selector, conversion, item, tree).Subscribe(x => observer.OnNext(x));
                        });
                        //foreach (var x in ToTree(collection, id_selector, parent_id_selector, conversion, item, tree))
                        //    yield return x;
                    });
            });
          
        }

        public static void Visit<T>(this T tree, Func<T, IEnumerable<T>> children, Action<T> action)
        {
            action(tree);
            foreach (var item in children(tree))
                Visit(item, children, action);
        }

        public static bool IsRoot(this IReadOnlyTree tree) => (tree as IGetParent<IReadOnlyTree>).Parent == null;

        public static bool IsLeaf<T>(this ITree<T> tree) => tree.Count() == 0;

        public static void Add<T>(this ITree<T> tree, T data)
        {
        }

        public static void Remove<T>(this ITree<T> tree, T data)
        {
        }

        public static ITree<T> Create<T>(T data)
        {
            return new Tree<T>(data);
        }

        public static ITree<T> Create<T>(T data, params ITree<T>[] items)
        {
            return new Tree<T>(data, items);
        }

        public static ITree<T> Create<T>(T data, params object[] items)
        {
            return new Tree<T>(data, items);
        }

        public static void Visit<T>(this ITree<T> tree, Action<ITree<T>> action)
        {
            action(tree);
            if (tree.HasItems)
                foreach (var item in tree as IEnumerable<ITree<T>>)
                    Visit(item, action);
        }

        public static ITree<T>? Match<T>(this ITree<T> tree, Predicate<ITree<T>> action)
        {
            if (action(tree))
            {
                return tree;
            }
            else if (tree.HasItems)
            {
                foreach (var item in tree as IEnumerable<ITree<T>>)
                {
                    if (Match(item, action) is ITree<T> sth)
                    {
                        return sth;
                    }
                }
            }

            return null;
        }



        public static ITree<T>? Match<T>(this ITree<T> tree, T data)
        {
            return Match(tree, a => a.Data?.Equals(data) == true);
        }

        public static ITree<T>? Match<T>(this ITree<T> tree, Guid guid)
        {
            return Match(tree, a => (a as IGetKey).Key.Equals(guid));
        }
    }
}