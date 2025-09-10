using ActivateAnything;
using Cogs.Collections;
using DynamicData;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Reactive.Linq;
using Utility.Changes;
using Utility.Helpers;
using Utility.Interfaces.Exs;
using Utility.Models.Diagrams;
using Utility.Observables.Generic;
using Utility.Reactives;
using IMethod = Utility.Interfaces.Exs.IMethod;

namespace Utility.Services
{
    public class ServiceResolver : IObservable<Set<IResolvableNode>>, IObservable<Set<IResolvableConnection>>, IObservable<Set<IObserver<object>>>, IObservable<Set<IObservable<object>>>, IServiceResolver
    {
        readonly Dictionary<System.Type, IMethodParameter> cache = [];
        readonly ObservableDictionary<IMethod, MethodNode> methodNodes = [];
        readonly ObservableCollection<IObservable<object>> observables = [];
        readonly ObservableCollection<IObserver<object>> observers = [];
        readonly ObservableCollection<MethodConnection> connections = [];

        public ServiceResolver()
        {
            methodNodes.DictionaryChanged += (s, e) =>
            {
                if (e.Action == NotifyDictionaryChangedAction.Add)
                {
                    foreach (var item in e.NewItems)
                    {
                        if (item.Value is MethodNode connection)
                        {
                            connection.next += () =>
                            {
                                foreach (var c in connections)
                                {
                                    c.StopTransfer();

                                }
                            };
                        }
                    }
                }
            };
        }

        public IReadOnlyDictionary<System.Type, IMethodParameter> Cache => cache;
        public IReadOnlyDictionary<IMethod, MethodNode> Nodes => methodNodes;
        public IReadOnlyCollection<MethodConnection> Connections => connections;

        public void Observe<TParam>(IObservable<object> observable) where TParam : IMethodParameter
        {
            if (get<TParam>() is IMethodParameter methodParameter)
            {
                if (get(methodParameter) is MethodNode cache)
                {
                    observables.Add(observable);
                    connections.Add(new MethodConnection(cache.InValues[methodParameter.Name], observable));
                }
            }
        }

        public void ReactTo<TParam>(IObserver<object> observer) where TParam : IMethodParameter
        {
            if (get<TParam>() is IMethodParameter methodParameter)
            {
                if (get(methodParameter) is MethodNode cache)
                {
                    observers.Add(observer);
                    connections.Add(new MethodConnection(observer, cache.OutValue));
                }
            }
        }

        public void Connect<TParam, TParam2>() where TParam : IMethodParameter where TParam2 : IMethodParameter
        {
            if (get<TParam>() is IMethodParameter methodParameter)
            {
                if (get<TParam2>() is IMethodParameter methodParameter2)
                {
                    if (get(methodParameter) is MethodNode cache)
                    {
                        if (get(methodParameter2) is MethodNode cache2)
                        {
                            connections.Add(new MethodConnection(cache2.InValues[methodParameter2.Name], cache.OutValue));
                        }
                    }
                }
            }
        }

        private IMethodParameter get<TService>()
        {
            return cache.Get(typeof(TService), a => (IMethodParameter)Activator.CreateInstance(a));
        }

        private IResolvableNode get(IMethodParameter methodParameter)
        {
            return methodNodes.Get(methodParameter.Method, a => new MethodNode(a.MethodInfo, a.Instance) { });
        }

        public IDisposable Subscribe(IObserver<Set<IResolvableNode>> observer)
        {
            var set = new Set<IResolvableNode>([.. this.methodNodes.Select(a => Changes.Change<IResolvableNode>.Add(a.Value))]);
            observer.OnNext(set);

            return methodNodes.Changes<IResolvableNode>().Subscribe(a => observer.OnNext(a));
        }
        public IDisposable Subscribe(IObserver<Set<IResolvableConnection>> observer)
        {
            var set = new Set<IResolvableConnection>([.. this.connections.Select(Changes.Change<IResolvableConnection>.Add)]);
            observer.OnNext(set);
            return connections.Changes<IResolvableConnection>().Subscribe(a => observer.OnNext(a));
        }
        public IDisposable Subscribe(IObserver<Set<IObservable<object>>> observer)
        {

            return observables.AndChanges<IObservable<object>>().Subscribe(a => observer.OnNext(a));
        }

        public IDisposable Subscribe(IObserver<Set<IObserver<object>>> observer)
        {

            return observers.AndChanges<IObserver<object>>().Subscribe(a => observer.OnNext(a));
        }
    }
}
