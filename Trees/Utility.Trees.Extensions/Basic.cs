namespace Utility.Trees.Extensions
{
    using System;
    using System.Collections.Specialized;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;
    using Utility.Changes;
    using Utility.Interfaces.Generic;
    using Utility.Interfaces.NonGeneric;
    using Utility.Reactives;
    using Utility.Trees;
    using Utility.Trees.Abstractions;
    using Type = Changes.Type;

    public static partial class Basic
    {
        public static void Foreach(this IReadOnlyTree parent, Action<IReadOnlyTree, int> action, int level = 0)
        {
            action(parent, level);
            ++level;
            foreach (IReadOnlyTree item in parent.Children)
            {
                item.Foreach(action, level);
            }
        }

        public static int Level(this IReadOnlyTree IReadOnlyTree)
        {
            int i = 0;
            while ((IReadOnlyTree as IGetParent<IReadOnlyTree>).Parent != null)
            {
                i++;
                IReadOnlyTree = (IReadOnlyTree as IGetParent<IReadOnlyTree>).Parent;
            }
            return i;
        }

        public static int Level(this IReadOnlyTree IReadOnlyTree, IReadOnlyTree parent)
        {
            int i = 0;
            while ((IReadOnlyTree as IGetParent<IReadOnlyTree>).Parent != parent)
            {
                i++;
                IReadOnlyTree = (IReadOnlyTree as IGetParent<IReadOnlyTree>).Parent;
                if (IReadOnlyTree == null)
                    throw new Exception("FD 444");
            }
            return i;
        }

        public static int IndexOf(this IReadOnlyTree tree, IReadOnlyTree _item)
        {
            int i = 0;
            foreach (var item in tree.Children)
            {
                if (item.Equals(_item))
                    return i;
                i++;
            }
            return -1;
        }

        public static ITree Create(object data)
        {
            return new Tree(data);
        }

        public static ITree Create(object data, params ITree[] items)
        {
            return new Tree(data, items);
        }

        public static ITree Create(object data, params object[] items)
        {
            return new Tree(data, items);
        }

        public static void VisitAncestors(this IReadOnlyTree tree, Action<IReadOnlyTree> action)
        {
            action(tree);
            if ((tree as IGetParent<IReadOnlyTree>).Parent is ITree parent)
                parent.VisitAncestors(action);
        }

        public static void VisitDescendants(this IReadOnlyTree tree, Action<IReadOnlyTree> action)
        {
            action(tree);

            foreach (var item in tree.Children)
            {
                if (item is IReadOnlyTree t)
                    t.VisitDescendants(action);
            }
        }

        public static IObservable<Change<IReadOnlyTree>> Changes(this ITree tree)
        {
            return Observable.Create<Change<IReadOnlyTree>>(observer =>
            {
                CompositeDisposable disposables = new();
                tree.CollectionChanged += (sender, args) =>
                {
                    if (args.Action == NotifyCollectionChangedAction.Add && args.NewItems != null)
                    {
                        foreach (IReadOnlyTree item in args.NewItems.Cast<IReadOnlyTree>())
                        {
                            (item as ISetParent<IReadOnlyTree>).Parent = tree;
                            observer.OnNext(new Change<IReadOnlyTree>(item, Type.Add));

                            if (item is ITree _tree)
                                _tree.Changes().Subscribe(observer).DisposeWith(disposables);
                        }
                    }
                    else if (args.Action != NotifyCollectionChangedAction.Move && args.OldItems != null)
                    {
                        foreach (var item in args.OldItems.Cast<Tree>())
                        {
                            item.Parent = null;
                            observer.OnNext(new Change<IReadOnlyTree>(item, Type.Remove));
                        }
                    }
                    //base.ItemsOnCollectionChanged(sender, args);
                };
                return disposables;
            });
        }

        public static async Task<IReadOnlyTree> ToClone(this ITree tree)
        {
            var clone = (ITree)(await tree.AsyncClone());

            CompositeDisposable disposables = new();
            tree.AndAdditions<ITree>().Subscribe(async item =>
            {
                var childClone = (ITree)(await item.ToClone());
                (childClone as ISetParent<IReadOnlyTree>).Parent = clone;
                clone.Add(childClone);
            });
            return clone;
        }

        public static IReadOnlyTree Simplify(this ITree tree)
        {
            var clone = new Tree((tree as IGetData).Data.ToString()) { Key = (tree as IGetKey).Key, };

            CompositeDisposable disposables = new();
            tree.AndAdditions<ITree>().Subscribe(async item =>
            {
                var childClone = (ITree)(item.Simplify());
                (childClone as ISetParent<IReadOnlyTree>).Parent = clone;
                clone.Add(childClone);
            });
            return clone;
        }

        public static bool IsRoot(this ITree tree)
        {
            return tree.Index.IsEmpty;
        }
    }
}