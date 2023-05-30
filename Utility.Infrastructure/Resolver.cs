using Castle.Core.Logging;
using DryIoc;
using DynamicData;
using NetFabric.Hyperlinq;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Reactive.Subjects;
using Utility.Interfaces.NonGeneric;
using Utility.Models;
using Utility.Observables.Generic;
using static System.Net.Mime.MediaTypeNames;

namespace Utility.Infrastructure
{
    public class Resolver : BaseObject
    {
        private readonly IContainer container;
        private IBase[] nodes;


        public ILogger Logger => container.Resolve<ILogger>();

        //private readonly History history;
        public override Key Key => new(Guids.Resolver, nameof(Resolver), typeof(Resolver));

        public ConcurrentDictionary<Guid, IObserverIOType> dictionary { get; } = new();

        public Resolver(IContainer container)
        {
            this.container = container;
            //history = container.Resolve<History>();
            //outputs = this.container.Resolve<Outputs[]>();
            nodes = this.container.Resolve<IBase[]>();

            Logger.Send(nodes);

            foreach (var node in nodes)
            {
                foreach (var keyValuePairs in node.GetSingleParameterMethods())
                {
                    dictionary[SingleParameterMethod.Combine(node.Key.Guid, keyValuePairs.Key)] = keyValuePairs.Value;
                }
            }
        }

        public void Initialise()
        {
            container.RegisterInitializer<object>((initialized, b) =>
            {
                if (initialized is IBase @base)
                {
                    Logger.Send(@base);
                }

                this.OnNext(new InitialisedEvent(initialized));
            });
            //broadcast(outputs);
            this.OnNext(new InitialisedEvent(this));
            //Broadcast(new InitialisedEvent(history));
        }

        public async void OnBase(IBase @base)
        {
            if (@base.Output is not GuidValue { Value: var iguid, Target: Guid target, Source: Guid source } guidValue)
            {
                if (@base.Output is not IGuid guid)
                {
                    throw new Exception("vdf2111 ww");
                }

                guidValue = new GuidValue(guid, @base.Key.Guid);
                source = guidValue.Source;
                target = guidValue.Target;
            }

            SynchronizationContext.SetSynchronizationContext(Context);

            if (dictionary.TryGetValue(SingleParameterMethod.Combine(source, target), out var observer))
            {
                await Logger.Send(guidValue, observer);

                if (observer is GuidBase { } guidBase)
                {
                    if (guidBase.IsComplete)
                    {
                        observer.OnCompleted();
                        if (dictionary.TryRemove(SingleParameterMethod.Combine(source, target), out var _) == false)
                        {

                        }
                        return;
                    }
                    else if (guidBase.Exception != null)
                    {
                        observer.OnError(guidBase.Exception);
                        return;
                    }
                    else if (guidBase.Progress is Progress { Amount: var amount, Total: var total })
                    {
                        observer.OnProgress(amount, total);
                        return;
                    }
                }
                observer.OnNext(guidValue);
                return;
            }
            else
            {
                foreach (var node in nodes)
                    if (dictionary.TryGetValue(SingleParameterMethod.Combine(node.Key.Guid, target), out var _observer))
                    {
                        await Logger.Send(guidValue, _observer);
                        SynchronizationContext.SetSynchronizationContext(Context);

                        _observer.OnNext(guidValue);
                        return;
                    }
            }
            if(@base.Output is GuidValue { Value: GuidBase { Exception:var exception } })
            {
                return;
            }
            if(@base.Output is GuidValue { Value: GuidBase { IsComplete:true} })
            {
                return;
            }
            if(@base.Output is GuidValue { Value: GuidBase { Progress:var progress } })
            {
                return;
            }
            if (@base.Output is not InitialisedEvent @event)
            {


            }
            //foreach (var connection in connections)
            //{
            //    Broadcast(@base, connection)
            //        .Subscribe();
            //}

            //foreach (var connection in connections.Where(a => a.IsPriority == false))
            //{
            //    var order = new Order(Guid.NewGuid(), @base, _value, connection);
            //    history.OnNext(order);
            //}
        }

        internal Utility.Interfaces.Generic.IObservable<TOutput> Register<TOutput, TInput>(BaseObject baseObject, TInput tInput) where TInput : IGuid
        {
            var guid = new GuidBase();
            var source = new GuidValue(tInput, baseObject.Key.Guid, new(guid, baseObject.Key.Guid));
            //var guidKey = new GuidKey(guid.Guid);

            var replay = new Utility.Observables.Generic.Subject<TOutput>();
            var subject = new CustomSubject<TInput, TOutput>(a =>
            {
                if (a.Value is GuidBase guidBase)
                {
                    if (guidBase.IsComplete)
                    {
                        dictionary.Remove(SingleParameterMethod.Combine(guid.Guid, typeof(TOutput).GUID), out var value);
                        replay.OnCompleted();
                    }
                    else if (guidBase.Exception is Exception ex)
                    {
                        replay.OnError(ex);
                    }
                    else if (guidBase.Progress is Progress progress)
                    {
                        replay.OnProgress(progress.Amount, progress.Total);
                    }
                }
                else
                    replay.OnNext((TOutput)a.Value);
                return default;
            });

            if (dictionary.ContainsKey(guid.Guid))
            {
                throw new InvalidOperationException("ds es");
            }
            dictionary[SingleParameterMethod.Combine(guid.Guid, typeof(TOutput).GUID)] = subject;

            baseObject.Output = source;
            OnBase(baseObject);
            return replay;
        }



        //public void OnNext(History history)
        //{
        //    if (history.Output is not ChangeSet { } changeSet)
        //    {
        //        throw new Exception("ujuj  sdsdf");
        //    }

        //    foreach (var connection in Connections(history.Key))
        //        Broadcast(history, changeSet, connection);

        //    foreach (var item in changeSet)
        //    {
        //        Broadcast(item);
        //    }
        //}

        //protected override void Broadcast(object obj)
        //{
        //    Output = obj;
        //    this.OnNext(this);
        //}

        //private void Broadcast(Change item)
        //{
        //    //if (item is not Change { Type: var type, Value: HistoryOrder { History: var hist, Order: var order } })
        //    //{
        //    //    throw new Exception("22 j  sdsdf");
        //    //}
        //    //switch (hist, type)
        //    //{
        //    //    case (Enums.History.Present, Models.ChangeType.Add):
        //    //        if (order is Order { Value: var value, Base: var @base, Connection: var connection })
        //    //        {
        //    //            Broadcast(@base, value, connection).Subscribe(a =>
        //    //            {
        //    //                //if (a == false)
        //    //                //    throw new Exception("no path for message");
        //    //            });
        //    //            break;
        //    //        }
        //    //        else
        //    //            throw new Exception("s44 dv3 ");
        //    //}
        //}

        //private IObservable<bool> Broadcast(IBase @base, IBase connection)
        //{
        //    //if (connection.Observers.Any() == false)
        //    //    throw new Exception("kl99dsf  sdffdsdff");
        //    //bool success = false;
        //    ReplaySubject<bool> subject = new(1);
        //    try
        //    {

        //        //if (connection.SkipContext == false || SynchronizationContext.Current ==null)
        //        //if (SynchronizationContext.Current != Context)
        //        //{
        //        //    Dispatch(() =>
        //        //    {

        //        connection.OnNext(@base.Output);

        //        if (@base.Output is GuidValue { Value: ObjectCreationResponse response })
        //        {
        //            isDirty = true;
        //        }
        //        //success |= result;
        //        //SpreadBroadcastEvent(@base, observer, result);

        //        //        subject.OnNext(success);
        //        //    });
        //        //}
        //        //else
        //        //{
        //        //    connection.OnNext(@base.Output);

        //        //    //foreach (var observer in connection.Outputs)
        //        //    //{
        //        //    //    observer.OnNext(order);
        //        //    //    //Broadcast(new BroadcastEvent(observer));
        //        //    //}
        //        //}
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    return subject;
        //}

        //private void SpreadBroadcastEvent(IBase @base, IObserver observer, bool result)
        //{
        //    foreach (var conn in Connections(this.Key))
        //    {
        //        foreach (var obs in conn.Outputs)
        //        {
        //            if (result)
        //                obs.OnNext(new BroadcastSuccessEvent(@base, observer));
        //            else
        //                obs.OnNext(new BroadcastFailureEvent(@base, observer));
        //        }
        //    }
        //}



        //ICollection<IBase> Connections(IEquatable equatable)
        //{
        //    if (equatable is not Key key)
        //    {
        //        throw new Exception("£ dfgdf");
        //    }
        //    if (isDirty)
        //    {
        //        nodes = this.container.Resolve<IBase[]>();
        //        isDirty = false;
        //    }
        //    Collection<IBase> observers = new();

        //    foreach (var outputConnection in outputs)
        //    {
        //        if (outputConnection.Connection.Match(key))
        //        {
        //            foreach (var x in outputConnection.Connections)
        //            {
        //                observers.AddRange(nodes.Where(a => x.Match(a.Key)));
        //            }
        //        }
        //    }
        //    return observers;
        //}
    }

    public class CustomSubject<TInput, TOutput> : Subject<GuidValue, TOutput>, IObserverIOType
    {
        public CustomSubject(Func<GuidValue, TOutput> onNext) : base(onNext)
        {
        }

        public override Type InType => typeof(TInput);

        public override Type OutType => typeof(TOutput);

    }
    //public record Order(Guid Key, IBase Base, object Value, IConnection Connection);
}
