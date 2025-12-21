using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using Utility.Attributes;
using Utility.Helpers.Reflection;
using Utility.Interfaces.Exs;
using Utility.Interfaces.Methods;
using Utility.Reactives;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Reflection;


namespace Utility.Models.Diagrams
{
    public class MethodConnection : IResolvableConnection
    {
        public static MethodConnection FromAction(Action<object> @in, IObservable<object> @out) => new(Observer.Create(@in), @out);

        public MethodConnection(IObserver<object> @in, IObservable<object> @out, Type? type = null)
        {
            Disposable = @out.Subscribe(a =>
            {
                Transfer?.Invoke();
                foreach (var @in in In)
                    Subscribe(a, @in, type);      
            });
            In = [@in];
            Out = @out;
        }

        public event Action Transfer;
        public event Action TransferComplete;

        public void StopTransfer()
        {
            TransferComplete?.Invoke();
        }

        public IDisposable Disposable { get; }
        public ObservableCollection<IObserver<object>> In { get; } = new ObservableCollection<IObserver<object>>();
        public IObservable<object> Out { get; }

        public static IDisposable Subscribe(object a, IObserver<object> action, Type type)
        {
            if (AttributeHelper.TryGetAttribute<ParamAttribute>(type, out var attr))
            {
                return listen(a, attr).Subscribe(_ =>
                {
                    action.OnNext(a);
                });
            }
            else if (action is MethodConnector { Parameter: ParameterInfo param} && a.GetType().IsAssignableTo(param.ParameterType))
            {
                action.OnNext(a);
            }
            else if (trySubscribe(a, action.OnNext, () => { }, e => Globals.Exceptions.OnNext(e)) is { } disposable)
            {
                return disposable;
            }
            else
            {
                throw new Exception("DFS£CDD£dkds");
            }
            return System.Reactive.Disposables.Disposable.Empty;

            IObservable<object> listen(object a, ParamAttribute attribute)
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
                        var sub = collectionChanged.NotificationChanges().Subscribe(async _ =>
                        {
                            await triggerIfChangedAsync((x, y) => false, cts.Token);
                        });
                        disposables.Add(sub);
                    }
                    else if (attribute.Event.HasFlag(Enums.CLREvent.PropertyChanged) &&
                        a is INotifyPropertyChanged propertyChanged)
                    {
                        PropertyChangedEventHandler handler = async (_, __) =>
                        {
                            await triggerIfChangedAsync((x, y) => false, cts.Token);
                        };
                        propertyChanged.PropertyChanged += handler;
                        disposables.Add(System.Reactive.Disposables.Disposable.Create(() => propertyChanged.PropertyChanged -= handler));
                    }
                    else if (attribute.Event.HasFlag(Enums.CLREvent.CustomEvent))
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
                            disposables.Add(System.Reactive.Disposables.Disposable.Create(() => eventInfo.RemoveEventHandler(a, handler)));
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

                        disposables.Add(System.Reactive.Disposables.Disposable.Create(() =>
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
            static IDisposable? trySubscribe(object obj, Action<object> action, Action? completed = null, Action<Exception>? aex = null)
            {
                var type = obj.GetType();

                // Find IObservable<T>
                var observableInterface = type
                    .GetInterfaces()
                    .FirstOrDefault(i =>
                        i.IsGenericType &&
                        i.GetGenericTypeDefinition() == typeof(IObservable<>));

                if (observableInterface == null)
                    return null; // not observable

                var t = observableInterface.GetGenericArguments()[0];

                // Create an Observer<T> wrapper that forwards values to an IObserver<object>
                var observerType = typeof(Utility.Reactives.Observer<>).MakeGenericType(t);
                Delegate convertedAction = Delegate.CreateDelegate(typeof(Action<>).MakeGenericType(t), action.Target, action.Method);

                var observer = Activator.CreateInstance(observerType, convertedAction, aex, completed);

                // Call Subscribe(observer)
                var subscribeMethod = observableInterface.GetMethod("Subscribe");
                var disposable = (IDisposable)subscribeMethod.Invoke(obj, new[] { observer });
                return disposable;
            }
        }
    }
}
