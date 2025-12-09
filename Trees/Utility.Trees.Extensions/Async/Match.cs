using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Utility.Changes;
using Utility.Interfaces.Generic;
using Utility.Observables.Generic;
using Utility.PropertyNotifications;
using Utility.Reactives;
using Utility.Trees.Abstractions;
using Type = Utility.Changes.Type;

namespace Utility.Trees.Extensions.Async
{
    public static class Match
    {

        public static IObservable<IReadOnlyTree> SelfAndDescendants(
     this IReadOnlyTree tree,
    Predicate<(IReadOnlyTree tree, int level)>? action = null,
    int level = 0)
        {
            action ??= _ => true;

            return Observable.Create<IReadOnlyTree>(observer =>
            {
                var disposables = new CompositeDisposable();

                observer.OnNext(tree);

                // Subscribe to descendants
                var changesSubscription = tree.Children
                     .AndAdditions<ITree>()
                     .Subscribe(item =>
                     {
                         SelfAndDescendants(item)
                         .Subscribe(observer.OnNext);
                     });


                disposables.Add(changesSubscription);

                //// Subscribe to children removals
                //var subtractionsSubscription = tree.Children.Subtractions<ITree>()
                //    .Subscribe(item =>
                //        observer.OnNext(new TreeChange<IReadOnlyTree>(item, null, Type.Remove, level + 1))
                //    );

                //disposables.Add(subtractionsSubscription);

                return disposables;
            });
        }


        //public static IObservable<TreeChange<IReadOnlyTree>> SelfAndDescendants(
        //    this IReadOnlyTree tree,
        //    Predicate<(IReadOnlyTree tree, int level)>? action = null,
        //    int level = 0)
        //{
        //    action ??= _ => true;

        //    return Observable.Create<TreeChange<IReadOnlyTree>>(observer =>
        //    {
        //        var disposables = new CompositeDisposable();

        //        if (action((tree, level)))
        //            observer.OnNext(new TreeChange<IReadOnlyTree>(tree, null, Type.Add, level));

        //        // Subscribe to descendants
        //        var descendantsSubscription = tree.Descendants(action, level + 1)
        //            .Subscribe(observer.OnNext);

        //        disposables.Add(descendantsSubscription);

        //        //// Subscribe to children removals
        //        //var subtractionsSubscription = tree.Children.Subtractions<ITree>()
        //        //    .Subscribe(item =>
        //        //        observer.OnNext(new TreeChange<IReadOnlyTree>(item, null, Type.Remove, level + 1))
        //        //    );

        //        //disposables.Add(subtractionsSubscription);

        //        return disposables;
        //    });
        //}


        //public static IObservable<IReadOnlyTree> Descendants(
        //    this IReadOnlyTree tree,
        //    Predicate<(IReadOnlyTree tree, int level)>? action = null,
        //    int level = 1)
        //{
        //    action ??= _ => true;

        //    return Observable.Create<IReadOnlyTree>(observer =>
        //    {
        //        var disposables = new CompositeDisposable();

        //        var changesSubscription = tree.Children
        //            .AndAdditions<ITree>()
        //            .Subscribe(item =>
        //            {
                        
        //                Match.SelfAndDescendants(item, action, level + 1)
        //                .Subscribe(observer.OnNext);
        //            });    
        //    disposables.Add(changesSubscription);

        //    return disposables;
        //});
        //}

        public static IObservable<TreeChange<IReadOnlyTree>> Descendants(this IReadOnlyTree tree,
            Predicate<(IReadOnlyTree tree, int level)>? action = null)
        {
            return Observable.Create<TreeChange<IReadOnlyTree>>(observer =>
            {
                CompositeDisposable disposables = [];
                tree.Children
                .AndChanges<ITree>()
                .Subscribe(set =>
                {
                    foreach (var item in set)
                    {
                        if (item.Type == Type.Add)
                        {
                            SelfAndDescendants(item.Value, action, 1)
                            .Subscribe(x =>
                            {
                                observer.OnNext(TreeChange<IReadOnlyTree>.Add(item.Value, 1));
                            }).DisposeWith(disposables);
                        }
                        else if (item.Type == Type.Remove)
                        {
                            observer.OnNext(TreeChange<IReadOnlyTree>.Remove(item.Value, 1));
                        }
                        else if (item.Type == Type.Update)
                        {
                            observer.OnNext(TreeChange<IReadOnlyTree>.Replace(item.Value, item.OldValue, 1));
                        }
                    }
                }).DisposeWith(disposables);

                return disposables;
            });
        }

        //public static IEnumerable<IReadOnlyTree> Descendant(
        //    this IReadOnlyTree tree,
        //    Predicate<(IReadOnlyTree tree, int level)>? action = null)
        //{
        //    return tree.Descendants(action).Take(1);
        //}

        public static IObservable<IReadOnlyTree> SelfAndAncestors(
            this IReadOnlyTree tree,
            Predicate<(IReadOnlyTree tree, int level)>? action = null,
            int level = 0)
        {
            action ??= _ => true;

            return Observable.Create<IReadOnlyTree>(observer =>
            {
                var disposables = new CompositeDisposable();

                if (action((tree, level)))
                    observer.OnNext(tree);

                ITree? parent = null;

                if (tree is IGetParent<ITree> treeWithParent)
                    parent = treeWithParent.Parent;
                else if (tree is IGetParent<IReadOnlyTree> treeWithReadonlyParent)
                    parent = treeWithReadonlyParent.Parent as ITree;

                if (parent != null)
                {
                    var parentSubscription = SelfAndAncestors(parent, action, level + 1)
                        .Subscribe(observer.OnNext);

                    disposables.Add(parentSubscription);
                }
                else if (tree is Tree changed)
                {
                    var changesSubscription = changed.WithChangesTo(a => a.Parent)
                        .Where(p => p != null)
                        .SelectMany(p => SelfAndAncestors(p, action, level + 1))
                        .Subscribe(observer.OnNext);

                    disposables.Add(changesSubscription);
                }

                return disposables;
            });
        }



        public static IObservable<IReadOnlyTree> Ancestors(
            this IReadOnlyTree tree,
            Predicate<(IReadOnlyTree tree, int level)>? action = null)
        {
            ITree? parent = (tree as IGetParent<IReadOnlyTree>)?.Parent as ITree;
            if (parent == null)
                return Observable.Empty<IReadOnlyTree>();

            return SelfAndAncestors(parent, action, 1);
        }
    }
}