using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Cogs.Collections;
using Utility.Attributes;
using Utility.Changes;
using Utility.Helpers;
using Utility.Helpers.Reflection;
using Utility.Interfaces.Exs;
using Utility.Interfaces.Methods;
using Utility.Observables.Generic;
using Utility.Reactives;
using IMethod = Utility.Interfaces.Methods.IMethod;

namespace Utility.Models.Diagrams
{
    public class ServiceResolver : IObservable<Set<IResolvableNode>>, IObservable<Set<IResolvableConnection>>, IObservable<Set<IObserver<object>>>, IObservable<Set<IObservable<object>>>, IServiceResolver
    {
        readonly Dictionary<System.Type, IParameter> cache = [];
        readonly ObservableCollection<MethodNode> methodNodes = [];
        readonly ObservableCollection<IObservable<object>> observables = [];
        readonly ObservableCollection<object> observers = [];
        readonly ObservableCollection<MethodConnection> connections = [];
        readonly Dictionary<IParameter, List<object>> reactions = [];

        public ServiceResolver()
        {
            methodNodes.CollectionChanged += (s, e) =>
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    foreach (var item in e.NewItems)
                    {
                        if (item is MethodNode connection)
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

        public IReadOnlyDictionary<System.Type, IParameter> Cache => cache;
        public IReadOnlyCollection<MethodNode> Nodes => methodNodes;
        public IReadOnlyCollection<MethodConnection> Connections => connections;

        public void Observe<TParam>(IObservable<object> observable) where TParam : IParameter
        {
            if (get<TParam>() is IParameter methodParameter)
            {
                if (get(methodParameter) is MethodNode cache)
                {
                    observables.Add(observable);
                    if (methodParameter.Parameter == null && cache.Parameters.Count == 0)
                    {
                        connections.Add(new MethodConnection(cache.InValues.Single().Value, observable, typeof(TParam)));
                        return;
                    }
                    IDisposable? disposable = null;
                    connections.Add(new MethodConnection(cache.InValues[methodParameter.Name], observable, typeof(TParam)));
                }
            }
        }

        public void ReactTo<TParam, TOutput>(IObserver<object> observer) where TParam : IParameter
        {
            if (get<TParam>() is IParameter methodParameter)
            {
                if (get(methodParameter) is MethodNode cache)
                {
                    observers.Add(observer);
                    if (connections.SingleOrDefault(a => a.Out is MethodConnector { Parameter: { } param } && param == methodParameter.Parameter) is { } connection)
                    {
                        connection.In.Add(observer);
                    }
                    else
                    {
                        connections.Add(new MethodConnection(observer, cache.OutValue, typeof(TParam)));
                    }
                }
            }
        }

        public void Connect<TParam, TParamOut>() where TParam : IParameter where TParamOut : IParameter
        {
            if (get<TParam>() is IParameter methodParameter)
            {
                if (get<TParamOut>() is IParameter methodParameterIn)
                {
                    if (get(methodParameter) is MethodNode cache)
                    {
                        if (get(methodParameterIn) is MethodNode cacheIn)
                        {
                            IDisposable? disposable = null;
                            connections.Add(new MethodConnection(cacheIn.InValues[methodParameterIn.Name], cache.OutValue, typeof(TParamOut)));
                        }
                    }
                }
            }
        }

        private IParameter get<TService>()
        {
            return cache.Get(typeof(TService), a => (IParameter)Activator.CreateInstance(a));
        }

        private IResolvableNode get(IParameter methodParameter)
        {
            var x = methodNodes.SingleOrDefault(a => a.MethodInfo == methodParameter.Method.MethodInfo);
            if (x == null)
                methodNodes.Add(x = MethodNode.Create(methodParameter.Method.MethodInfo, methodParameter.Method.Instance));
            return x;
        }

        public IDisposable Subscribe(IObserver<Set<IResolvableNode>> observer)
        {
            var set = new Set<IResolvableNode>([.. methodNodes.Select(a => Changes.Change.Add<IResolvableNode>(a))]);
            observer.OnNext(set);

            return methodNodes.Changes<IResolvableNode>().Subscribe(a => observer.OnNext(a));
        }

        public IDisposable Subscribe(IObserver<Set<IResolvableConnection>> observer)
        {
            var set = new Set<IResolvableConnection>([.. connections.Select(Changes.Change.Add<IResolvableConnection>)]);
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
