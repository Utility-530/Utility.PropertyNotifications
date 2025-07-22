using Cogs.Collections;
using DynamicData;
using Splat;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Reactive.Linq;
using Utility.Changes;
using Utility.Helpers;
using Utility.Interfaces.NonGeneric;
using Utility.Models;
using Utility.Observables.Generic;
using Utility.PropertyNotifications;
using Utility.Reactives;

namespace Utility.Services
{

    public class ServiceResolver : IObservable<Set<IResolvableNode>>, IObservable<Set<IResolvableConnection>>
    {
        readonly Dictionary<System.Type, IMethodParameter> cache = [];
        readonly ObservableDictionary<Method, MethodNode> methodNodes = [];
        readonly ObservableCollection<IResolvableNode> propertyNodes = [];
        readonly ObservableCollection<MethodConnection> connections = [];
        readonly Dictionary<IValueModel, IObservable<object>> dictionary = [];

        public ServiceResolver()
        {
        }
        public IReadOnlyDictionary<System.Type, IMethodParameter> Cache => cache;
        public IReadOnlyDictionary<Method, MethodNode> Nodes => methodNodes;
        public IReadOnlyCollection<MethodConnection> Connections => connections;

        public void Observe<TParam>(IValueModel tModel) where TParam : IMethodParameter
        {

            if (get<TParam>() is IMethodParameter methodParameter)
            {

                if (get(methodParameter) is MethodNode cache)
                {
                    var observable = tModel.WhenReceivedFrom(a => (a as IValue).Value, includeNulls: false)
                        .Merge(tModel.WithChangesTo(a => (a as IValue).Value, includeNulls: false));
                    if (tModel is not IGetName name)
                    {
                        throw new Exception("f 333333");
                    }
                    var node = new ObservableNode(name.Name, observable, tModel);
                    propertyNodes.Add(node);
                    var dis = observable
                        .Subscribe(value =>
                        {
                            cache.InValues[methodParameter.Name].Value = value;
                        });

                    connections.Add(new MethodConnection(cache.InValues[methodParameter.Name], observable, dis));
                }
            }
        }

        public void ReactTo<TParam>(IValueModel tModel, Func<object, object>? transformation = null, Action<object>? setAction = null) where TParam : IMethodParameter
        {
            setAction ??= (a) => (tModel as ISetValue).Value = a;
            if (get<TParam>() is IMethodParameter methodParameter)
            {
                if (get(methodParameter) is MethodNode cache)
                {
                    var observer = new Reactives.Observer<object>(a => setAction(transformation != null ? transformation.Invoke(a) : a), e => { }, () => { });
                    if (tModel is not IGetName name)
                    {
                        throw new Exception("f 333333");
                    }
                    var observerNode = new ObserverNode(name.Name, observer, tModel);
                    propertyNodes.Add(observerNode);
                    var dis = cache.OutValue.Subscribe(observer);
                    connections.Add(new MethodConnection(observer, cache.OutValue, dis));
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
                            var dis = cache.OutValue
                                .Subscribe(a => cache2.InValues[methodParameter2.Name].Value = a);
                            connections.Add(new MethodConnection(cache2.InValues[methodParameter2.Name], cache.OutValue, dis));
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
            return methodNodes.Get(methodParameter.Method, a => new MethodNode(a) { });
        }

        public IDisposable Subscribe(IObserver<Set<IResolvableNode>> observer)
        {
            var set = new Set<IResolvableNode>([.. this.methodNodes.Select(a => Changes.Change<IResolvableNode>.Add(a.Value))]);
            observer.OnNext(set);

            var set2 = new Set<IResolvableNode>([.. this.propertyNodes.Select(Changes.Change<IResolvableNode>.Add)]);
            observer.OnNext(set2);

            return methodNodes.Changes<IResolvableNode>().Merge((propertyNodes as INotifyCollectionChanged).Changes<IResolvableNode>()).Subscribe(a => observer.OnNext(a));
        }

        public IDisposable Subscribe(IObserver<Set<IResolvableConnection>> observer)
        {
            var set = new Set<IResolvableConnection>([.. this.connections.Select(Changes.Change<IResolvableConnection>.Add)]);
            observer.OnNext(set);
            return methodNodes.Changes<IResolvableConnection>().Subscribe(a => observer.OnNext(a));
        }
    }

    public static class ServiceHelper
    {
        public static IValueModel ToValueModel<T>(this IObservable<T> observable)
        {
            var valueModel = new ValueModel<T>() { Name = typeof(T).Name };
            observable.Subscribe(a => valueModel.Value = a);
            return valueModel;
        }

        public static IObservable<T> Observe<TParam, T>(this IObservable<T> observable) where TParam : IMethodParameter
        {
            observable.ToValueModel().Observe<TParam>();
            return observable;
        }

        public static void Observe<TParam>(this IValueModel tModel) where TParam : IMethodParameter =>
            Locator.Current.GetService<ServiceResolver>().Observe<TParam>(tModel);

        public static void ReactTo<TParam>(this IValueModel tModel, Func<object, object>? transformation = null, Action<object>? setAction = null) where TParam : IMethodParameter =>
            Locator.Current.GetService<ServiceResolver>().ReactTo<TParam>(tModel, transformation, setAction);
    }
}
