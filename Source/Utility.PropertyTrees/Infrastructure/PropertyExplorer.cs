using System.Collections.Specialized;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Utility.Nodes;
using Utility.Observables.NonGeneric;

namespace Utility.PropertyTrees.Infrastructure
{
    public static class PropertyExplorer
    {
        public enum State
        {
            Started, Completed
        }

        public static IObservable<(int, int)> ExploreTree<T>(T items, Func<T, PropertyBase, T> func, ValueNode property, out IDisposable disposable)
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

            _ = ExploreTree(items, func, property, subject);

            return progress;
        }

        public static IObservable<(int, int)> ExploreTree(ValueNode propertyNode, out IDisposable disposable)
        {
            return ExploreTree(new List<object>(), (a, b) => a, propertyNode, out disposable);

        }


        public static IDisposable ExploreTree<T>(T items, Func<T, PropertyBase, T> func, ValueNode property, Subject<State> state)
        {
            state.OnNext(State.Started);

            if (property is PropertyBase @base)
                items = func(items, @base);

            var disposable = property
                .Children
                .Subscribe(async item =>
                {
                    if (item is not NotifyCollectionChangedEventArgs args)
                        throw new Exception("rev re");

                    state.OnNext(State.Started);
                    foreach (PropertyBase node in SelectNewItems<PropertyBase>(args))
                    {
                        _ = ExploreTree(items, func, node, state);
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



        public static IObservable<ValueNode> FindNode(ValueNode node, Predicate<ValueNode> predicate, out IDisposable disposable)
        {
            ReplaySubject<ValueNode> list = new(1);
            System.Reactive.Disposables.CompositeDisposable composite = new();
            ExploreTree(list, (a, b) => { if (predicate(b)) { a.OnNext(b); composite.Dispose(); a.OnCompleted(); } return (a); }, node, out IDisposable _dis)
                .Subscribe(a =>
                {
                }, list.OnCompleted).DisposeWith(composite);
            composite.Add(_dis);
            disposable = composite;
            return list;
        }

        public static IObservable<ValueNode> FindNodes(ValueNode node, Predicate<ValueNode> predicate)
        {
            Subject<ValueNode> list = new();

            ExploreTree(list, (a, b) => { if (predicate(b)) a.OnNext(b); return a; }, node, out IDisposable disposable).Subscribe(a =>
            {
            }, list.OnCompleted);
            return list;
        }

        private static System.Collections.Generic.IEnumerable<T> SelectNewItems<T>(NotifyCollectionChangedEventArgs args)
        {
            return args.NewItems?.Cast<T>() ?? Array.Empty<T>();
        }
    }
}


