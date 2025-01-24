using System.Reactive.Disposables;
using System.Reactive.Linq;
using Utility.Changes;
using Utility.Trees.Abstractions;
using Utility.Reactives;
using Type = Utility.Changes.Type;
using Utility.PropertyNotifications;

namespace Utility.Trees.Extensions.Async
{
    public static class Match
    {
        public static IObservable<TreeChange<IReadOnlyTree>> SelfAndDescendants(this IReadOnlyTree tree, Predicate<(IReadOnlyTree tree, int level)>? action = null, int level = 0)
        {
            return Observable.Create<TreeChange<IReadOnlyTree>>(observer =>
            {
                CompositeDisposable disposables = new CompositeDisposable();
                action ??= n => true;
                if (action((tree, level)))
                {
                    observer.OnNext(new(tree, null, Type.Add, level));
                }
                else
                {

                }
                level++;

                tree.Items.AndAdditions<ITree>()
                .Subscribe(item =>
                {
                    SelfAndDescendants(item, action, level)
                    .Subscribe(x =>
                    {
                        observer.OnNext(x);
                    }).DisposeWith(disposables);

                }).DisposeWith(disposables);


                tree.Items.Subtractions<ITree>()
                .Subscribe(item =>
                {
                    observer.OnNext(new(item, null, Type.Remove, level));
                }).DisposeWith(disposables);


                return disposables;
            });
        }

        public static IObservable<IReadOnlyTree> SelfAndAncestors(this IReadOnlyTree tree, Predicate<(IReadOnlyTree tree, int level)>? action = null, int level = 0)
        {
            return Observable.Create<IReadOnlyTree>(observer =>
            {
                CompositeDisposable disposables = new();
                action ??= n => true;
                if (action((tree, level)))
                {
                    observer.OnNext(tree);
                }

                if (tree?.Parent is ITree parent)
                {
                    return SelfAndAncestors(parent, action, level++)
                        .Subscribe(a => observer.OnNext(a))
                        .DisposeWith(disposables);
                }
                else
                {
                    if (tree is Tree changed)
                        return changed.WithChangesTo(a => a.Parent)
                        .Where(a => a != null)
                        .Subscribe(p =>
                        {
                            SelfAndAncestors(p, action, level++)
                            .Subscribe(a =>
                            observer.OnNext(a))
                            .DisposeWith(disposables);
                        })
                        .DisposeWith(disposables);
                    else if (tree.Parent != null)
                    {
                        return SelfAndAncestors(tree.Parent, action, level++)
                      .Subscribe(a =>
                      observer.OnNext(a));
                    }
                    else
                    {
                        throw new Exception("SD 333333 9d");
                    }
                }
            });
        }

        public static IObservable<IReadOnlyTree> Ancestors(this IReadOnlyTree tree, Predicate<(IReadOnlyTree tree, int level)>? action = null)
        {
            return SelfAndAncestors(tree.Parent, action);
        }

        public static IObservable<TreeChange<IReadOnlyTree>> Descendants(this IReadOnlyTree tree, Predicate<(IReadOnlyTree tree, int level)>? action = null)
        {
            return Observable.Create<TreeChange<IReadOnlyTree>>(observer =>
            {
                CompositeDisposable disposables = [];
                tree.Items.AndChanges<ITree>()
                .Subscribe(item =>
                {
                    foreach (var _o in item)
                    {
                        if (_o.Type == Type.Add)
                        {
                            SelfAndDescendants(_o.Value, action, 1)
                            .Subscribe(x =>
                            {
                                observer.OnNext(x);
                            }).DisposeWith(disposables);
                            observer.OnNext(TreeChange<IReadOnlyTree>.Add(_o.Value, 1));
                        }
                        else if (_o.Type == Type.Remove)
                        {
                            observer.OnNext(TreeChange<IReadOnlyTree>.Remove(_o.Value, 1));
                        }
                    }
                }).DisposeWith(disposables);

                return disposables;
            });
        }

        public static IObservable<TreeChange<IReadOnlyTree>> Descendant(this IReadOnlyTree tree, Predicate<(IReadOnlyTree tree, int level)>? action = null)
        {
            return Descendants(tree, action).Take(1);
        }
    }
}

