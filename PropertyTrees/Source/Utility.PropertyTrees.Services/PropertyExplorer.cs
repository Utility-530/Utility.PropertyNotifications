using System.Collections.Specialized;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Utility.Nodes;
using Utility.Trees.Abstractions;
using Utility.Observables.NonGeneric;
using Utility.Helpers.Ex;
using System.Collections;

namespace Utility.PropertyTrees.Services
{
    public static class PropertyExplorer
    {
        public enum State
        {
            Started, Completed
        }

        public static IObservable<(int, int)> ExploreTree<T>(T items, Func<T, IReadOnlyTree, T> funcAdd, Action<T, IReadOnlyTree> funcRemove, IReadOnlyTree property, out IDisposable disposable)
        {
            Subject<State> subject = new();
            int totalCount = 1;
            int completed = 1;
            Subject<(int, int)> progress = new();
            disposable = subject
                 .Subscribe(a =>
                 {
                     if (a == State.Started)
                         totalCount++;
                     else if (a == State.Completed)
                         completed++;

                     progress.OnNext((completed, totalCount));

                     if (totalCount - completed == 0)
                     {
                         progress.OnCompleted();
                     }
                 });

            _ = ExploreTree(items, funcAdd, funcRemove, property, subject);

            return progress;
        }

        public static IObservable<(int, int)> ExploreTree(IReadOnlyTree propertyNode, out IDisposable disposable)
        {
            return ExploreTree(new List<object>(), (a, b) => a, (a, b) => { }, propertyNode, out disposable);

        }

        public static IDisposable ExploreTree<T>(T items, Func<T, IReadOnlyTree, T> funcAdd, Action<T, IReadOnlyTree> funcRemove, IReadOnlyTree property, Subject<State> state)
        {
            state.OnNext(State.Started);

            if (property is IReadOnlyTree @base)
                items = funcAdd(items, @base);

            var disposable = property
                .Items
                .SelectExistingItemsAndChanges()
                .Subscribe(async args =>
                {
                    state.OnNext(State.Started);
                    foreach (IReadOnlyTree node in SelectNewItems<IReadOnlyTree>(args))
                    {
                        _ = ExploreTree(items, funcAdd, funcRemove, node, state);
                    }
                    foreach (IReadOnlyTree node in SelectOldItems<IReadOnlyTree>(args))
                    {
                        funcRemove(items, node);
                        //_ = ExploreTree(items, func, node, state);
                    }
                    state.OnNext(State.Completed);
                },
                e =>
                {

                },
                () =>
                {
                    state.OnNext(State.Completed);
                }
              );
            return disposable;
        }

        public static IObservable<IReadOnlyTree> FindNode(IReadOnlyTree node, Predicate<IReadOnlyTree> predicate, out IDisposable disposable)
        {
            ReplaySubject<IReadOnlyTree> list = new(1);
            CompositeDisposable composite = new();
            ExploreTree(list, (a, b) => { if (predicate(b)) { a.OnNext(b); composite.Dispose(); a.OnCompleted(); } return a; }, (a,b)=> { }, node, out IDisposable _dis)
                .Subscribe(a =>
                {
                }, list.OnCompleted).DisposeWith(composite);
            composite.Add(_dis);
            disposable = composite;
            return list;
        }

        public static IObservable<IReadOnlyTree> FindNodes(IReadOnlyTree node, Predicate<IReadOnlyTree> predicate)
        {
            Subject<IReadOnlyTree> list = new();

            ExploreTree(list, (a, b) => { if (predicate(b)) a.OnNext(b); return a; }, (a, b) => { }, node, out IDisposable disposable).Subscribe(a =>
            {
            }, list.OnCompleted);
            return list;
        }

        private static IEnumerable<T> SelectNewItems<T>(NotifyCollectionChangedEventArgs args)
        {
            return args.NewItems?.Cast<T>() ?? Array.Empty<T>();
        }

        private static IEnumerable<T> SelectOldItems<T>(NotifyCollectionChangedEventArgs args)
        {
            return args.OldItems?.Cast<T>() ?? Array.Empty<T>();
        }
    }
}


