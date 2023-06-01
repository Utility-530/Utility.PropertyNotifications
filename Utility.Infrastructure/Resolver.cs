using DryIoc;
using DynamicData;
using System.Collections.Concurrent;
using Utility.Interfaces.NonGeneric;
using Utility.Models;
using Utility.Observables.Generic;

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

                if (guidValue.Value is GuidBase { } guidBase)
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
            if(@base.Output is GuidValue { Value: GuidBase { Exception: var exception } })
            {
                return;
            }
            if(@base.Output is GuidValue { Value: GuidBase { IsComplete: true} })
            {
                return;
            }
            if(@base.Output is GuidValue { Value: GuidBase { Progress: var progress } })
            {
                return;
            }
            if (@base.Output is not InitialisedEvent @event)
            {


            }

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

        public void Clear()
        {
            foreach(var keyValuePair in dictionary) { 
            
                if(keyValuePair.Value is SingleParameterMethod singleParameterMethod)
                {
                    foreach (var disposable in singleParameterMethod.disposables)
                        disposable.Dispose();
                }
                else
                {
                    dictionary.Remove(keyValuePair.Key, out var observer);
                    if(observer is Utility.Observables.Generic.Subject subject)
                    {
                        subject.Outputs.Clear();
                        subject.Observers.Clear();
                    }
                }
            }
        }
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
