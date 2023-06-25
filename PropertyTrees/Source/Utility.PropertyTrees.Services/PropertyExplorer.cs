using System.Collections.Specialized;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Utility.Nodes;
using Utility.Nodes.Abstractions;
using Utility.Observables.NonGeneric;

namespace Utility.PropertyTrees.Services
{
    public static class PropertyExplorer
    {
        public enum State
        {
            Started, Completed
        }

        public static IObservable<(int, int)> ExploreTree<T>(T items, Func<T, INode, T> funcAdd, Action<T, INode> funcRemove, INode property, out IDisposable disposable)
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

        public static IObservable<(int, int)> ExploreTree(INode propertyNode, out IDisposable disposable)
        {
            return ExploreTree(new List<object>(), (a, b) => a, (a, b) => { }, propertyNode, out disposable);

        }

        public static IDisposable ExploreTree<T>(T items, Func<T, INode, T> funcAdd, Action<T, INode> funcRemove, INode property, Subject<State> state)
        {
            state.OnNext(State.Started);

            if (property is INode @base)
                items = funcAdd(items, @base);

            var disposable = property
                .Children
                .Subscribe(async item =>
                {
                    if (item is not NotifyCollectionChangedEventArgs args)
                        throw new Exception("rev re");

                    state.OnNext(State.Started);
                    foreach (INode node in SelectNewItems<INode>(args))
                    {
                        _ = ExploreTree(items, funcAdd, funcRemove, node, state);
                    }
                    foreach (INode node in SelectOldItems<INode>(args))
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

        public static IObservable<INode> FindNode(INode node, Predicate<INode> predicate, out IDisposable disposable)
        {
            ReplaySubject<INode> list = new(1);
            CompositeDisposable composite = new();
            ExploreTree(list, (a, b) => { if (predicate(b)) { a.OnNext(b); composite.Dispose(); a.OnCompleted(); } return a; }, (a,b)=> { }, node, out IDisposable _dis)
                .Subscribe(a =>
                {
                }, list.OnCompleted).DisposeWith(composite);
            composite.Add(_dis);
            disposable = composite;
            return list;
        }

        public static IObservable<INode> FindNodes(INode node, Predicate<INode> predicate)
        {
            Subject<INode> list = new();

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


