using System.Collections.Specialized;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using Utility.Observables;

namespace Utility.PropertyTrees.Infrastructure
{
    public static class PropertyHelper
    {
        public enum State
        {
            Started, Completed
        }

        public static IObservable<double> ExploreTree<T>(T items, Func<T, PropertyBase, T> func, PropertyNode property)
        {
            Subject<State> subject = new();
            int totalCount = 1;
            int completed = 1;
            Subject<double> progress = new();
            subject
                 .Subscribe(a =>
                 {
                     if (a == State.Started)
                         totalCount++;
                     else if (a == State.Completed)
                         completed++;

                     progress.OnNext(completed / (1d * totalCount));

                     if (totalCount - completed == 0)
                     {
                         progress.OnCompleted();
                     }
                 });

            _ = ExploreTree(items, func, property, subject);

            return progress;
        }

        public static IObservable<double> ExploreTree(PropertyNode propertyNode)
        {
            return ExploreTree(new List<object>(), (a, b) => a, propertyNode);
           
        }


        public static IDisposable ExploreTree<T>(T items, Func<T, PropertyBase, T> func, PropertyNode property, Subject<State> state)
        {
            state.OnNext(State.Started);
            var disposable = property
                .Children
                .Subscribe(async item =>
                {
                    if (item is not NotifyCollectionChangedEventArgs args)
                        throw new Exception("rev re");
                    var p = property;

                    state.OnNext(State.Started);
                    foreach (PropertyBase node in SelectNewItems<PropertyBase>(args))
                    {
                        _ = ExploreTree(func(items, node), func, node, state);
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



        public static IObservable<PropertyNode> FindNode(PropertyNode node, Predicate<PropertyNode> predicate)
        {
            ReplaySubject<PropertyNode> list = new(1);
            CompositeDisposable composite = new();
            var dis = ExploreTree(list, (a, b) => { if (predicate(b)) { a.OnNext(b); composite.Dispose(); a.OnCompleted(); } return (a); }, node).Subscribe(a =>
            {
            }, list.OnCompleted);
            composite.Add(dis);
            return list;
        }

        public static IObservable<PropertyNode> FindNodes(PropertyNode node, Predicate<PropertyNode> predicate)
        {
            Subject<PropertyNode> list = new();

            ExploreTree(list, (a, b) => { if (predicate(b)) a.OnNext(b); return a; }, node).Subscribe(a =>
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


