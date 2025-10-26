using Cogs.Collections;
using DynamicData;
using LanguageExt.ClassInstances;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
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
        readonly ObservableDictionary<IMethod, MethodNode> methodNodes = [];
        readonly ObservableCollection<IObservable<object>> observables = [];
        readonly ObservableCollection<object> observers = [];
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

        public IReadOnlyDictionary<System.Type, IParameter> Cache => cache;
        public IReadOnlyDictionary<IMethod, MethodNode> Nodes => methodNodes;
        public IReadOnlyCollection<MethodConnection> Connections => connections;

        public void Observe<TParam>(IObservable<object> observable) where TParam : IParameter
        {
            if (get<TParam>() is IParameter methodParameter)
            {
                if (get(methodParameter) is MethodNode cache)
                {
                    observables.Add(observable);
                    if (methodParameter.Info == null && cache.Parameters.Count == 0)
                    {
                        connections.Add(new MethodConnection(a =>
                        subscribe<TParam>(a, cache.InValues.Single().Value.OnNext), observable));
                        return;
                    }
                    IDisposable? disposable = null;
                    connections.Add(new MethodConnection(a =>
                    {
                        if (disposable != null)
                            disposable.Dispose();
                        disposable = subscribe<TParam>(a, cache.InValues[methodParameter.Name].OnNext);
                    }, observable));
                }
            }
        }

        public void ReactTo<TParam, TOutput>(IObserver<TOutput> observer) where TParam : IParameter
        {
            if (get<TParam>() is IParameter methodParameter)
            {
                if (get(methodParameter) is MethodNode cache)
                {
                    observers.Add(observer);

                    connections.Add(new MethodConnection(async a =>
                    {
                        convert<TOutput>(a).Subscribe(observer.OnNext);
                    }, cache.OutValue));
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
                            connections.Add(new MethodConnection(a =>
                            {
                                if (disposable != null)
                                    disposable.Dispose();
                                disposable = subscribe<TParamOut>(a, cacheIn.InValues[methodParameterIn.Name].OnNext);
                            }, cache.OutValue));
                        }
                    }
                }
            }
        }

        private IDisposable subscribe<TParamOut>(object a, Action<object> action) where TParamOut : IParameter
        {
            if (AttributeHelper.TryGetAttribute<ParamAttribute>(typeof(TParamOut), out var attr))
            {
                return listen(a, attr).Subscribe(_ =>
                {
                    action(a);
                });
            }
            action(a);
            return Disposable.Empty;
        }

        private IParameter get<TService>()
        {
            return cache.Get(typeof(TService), a => (IParameter)Activator.CreateInstance(a));
        }

        private IResolvableNode get(IParameter methodParameter)
        {
            return methodNodes.Get(methodParameter.Method, a => new MethodNode(a.MethodInfo, a.Instance) { });
        }

        public IDisposable Subscribe(IObserver<Set<IResolvableNode>> observer)
        {
            var set = new Set<IResolvableNode>([.. methodNodes.Select(a => Changes.Change<IResolvableNode>.Add(a.Value))]);
            observer.OnNext(set);

            return methodNodes.Changes<IResolvableNode>().Subscribe(a => observer.OnNext(a));
        }
        public IDisposable Subscribe(IObserver<Set<IResolvableConnection>> observer)
        {
            var set = new Set<IResolvableConnection>([.. connections.Select(Changes.Change<IResolvableConnection>.Add)]);
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

        private static IObservable<TOutput> convert<TOutput>(object a)
        {
            IObservable<TOutput> input = Observable.Empty<TOutput>();
            if (a is Task<TOutput> task)
                input = task.ToObservable();
            else if (a is Task { Status: TaskStatus.Created } _task)
            {
                _task.Start();
            }
            else if (a is IObservable<TOutput> observable)
                input = observable;
            else
                input = Observable.Return((TOutput)a);
            return input;

        }

        private IObservable<object> listen(object a, ParamAttribute attribute)
        {
            TimeSpan interval = TimeSpan.FromMinutes(1.0 / attribute.RatePerMinute);

            return Observable.Create<object>(observer =>
            {
                var cts = new CancellationTokenSource();
                object? lastSnapshot = null;
                var disposables = new CompositeDisposable();


                if (attribute.Event.HasFlag(Enums.CLREvent.CollectionChanged) &&
                    a is INotifyCollectionChanged collectionChanged)
                {
                    var sub = collectionChanged.Changes().Subscribe(async _ =>
                    {
                        await triggerIfChangedAsync((x, y) => false, cts.Token);
                    });
                    disposables.Add(sub);
                }

                if (attribute.Event.HasFlag(Enums.CLREvent.PropertyChanged) &&
                    a is INotifyPropertyChanged propertyChanged)
                {
                    PropertyChangedEventHandler handler = async (_, __) =>
                    {
                        await triggerIfChangedAsync((x, y) => false, cts.Token);
                    };
                    propertyChanged.PropertyChanged += handler;
                    disposables.Add(Disposable.Create(() => propertyChanged.PropertyChanged -= handler));
                }

                if (attribute.Event.HasFlag(Enums.CLREvent.CustomEvent))
                {
                    if (attribute.CustomEventName == null)
                        throw new ArgumentException("CustomEventName must be provided when using CustomEvent flag.");
                    if (a.GetType().GetEvent(attribute.CustomEventName) is { } eventInfo)
                    {
                        EventHandler handler = async (_, __) =>
                        {
                            await triggerIfChangedAsync((x, y) => false, cts.Token);
                        };
                        eventInfo.AddEventHandler(a, handler);
                        disposables.Add(Disposable.Create(() => eventInfo.RemoveEventHandler(a, handler)));
                    }
                }

                // Fallback to polling if no event types matched
                if (disposables.Count == 0)
                {
                    var pollingTask = Task.Run(async () =>
                    {
                        while (!cts.Token.IsCancellationRequested)
                        {
                            await triggerIfChangedAsync((x, y) => false, cts.Token);
                        }
                    }, cts.Token);

                    disposables.Add(Disposable.Create(() =>
                    {
                        cts.Cancel();
                        cts.Dispose();
                        _ = pollingTask.ContinueWith(t => _ = t.Exception, TaskContinuationOptions.OnlyOnFaulted);
                    }));
                }

                return disposables;

                async Task triggerIfChangedAsync(Func<object, object, bool> equals, CancellationToken token)
                {
                    try
                    {                  
                        if (token.IsCancellationRequested) return;

                        if (!equals(a, lastSnapshot))
                        {
                            lastSnapshot = a;
                            observer.OnNext(a);
                        }
                        await Task.Delay(interval, token);
                    }
                    catch (TaskCanceledException) { }
                }

            });
        }

    }
}
