using DryIoc;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using T;
using Utility.Infrastructure;
using Utility.Interfaces.NonGeneric;
using Utility.Models;
using Utility.Observables.Generic;

namespace Utility.PropertyTrees.Services
{
    public class Resolver : BaseObject, Utility.Infrastructure.IResolver
    {
        private readonly TaskFactory factory;
        private readonly IContainer container;
        private BaseObject[] nodes;

        Subject<Key> completedSubject = new();
        Subject<(Key, Exception)> exceptionSubject = new();
        Subject<(Key, Progress)> progressSubject = new();

        BlockingCollection<IObserverIOType> observers = new();
        BlockingCollection<IObserverIOType> completed = new();
        public ILogger Logger => container.Resolve<ILogger>();
        TaskScheduler _taskSched = new ConstrainedTaskScheduler() { MaximumTaskCount = 1 };
        //private readonly History history;
        public override Key Key => new(Guids.Resolver, nameof(Services.Resolver), typeof(Resolver));

        public IList<IObserverIOType> dictionary => this.container.Resolve<IObserverIOType[]>();

        public Resolver(IContainer container)
        {
            factory = new TaskFactory(_taskSched);
            this.container = container;
            //history = container.Resolve<History>();
            //outputs = this.container.Resolve<Outputs[]>();
            nodes = this.container.Resolve<BaseObject[]>();

            Logger.Send(nodes);

            foreach (var node in nodes)
            {
                foreach (var spMethod in node.GetSingleParameterMethods(this))
                {
                    //dictionary[SingleParameterMethod.Combine(node.Key.Guid, keyValuePairs.Key)] = keyValuePairs.Value;
                    //container.RegisterInstance<IObserverIOType>(spMethod);
                    observers.Add(spMethod);
                    Logger.Add(spMethod).Wait();
                }
            }

            completedSubject
                .Subscribe(obs =>
                {
                    var nodes = this.container.Resolve<BaseObject[]>();
                    var single = observers.Single(a => a.Key == obs);
                    completed.Add(single);
                });

            exceptionSubject
      .Subscribe(obs =>
      {
          //var nodes = this.container.Resolve<BaseObject[]>();
          //var single = observers.Single(a => a.Key == obs);
          //completed.Add(single);
      });

            progressSubject
      .Subscribe(obs =>
      {

      });
        }

        public void Initialise()
        {
            container.RegisterInitializer<object>((initialized, b) =>
            {
                if (initialized is BaseObject @base)
                {
                    Logger.Send(@base);
                }

                //  OnNext(new InitialisedEvent(initialized));
            });

            //  OnNext(new InitialisedEvent(this));
        }

        public void Send(GuidValue guidValue)
        {
            SynchronizationContext.SetSynchronizationContext(Context);

            //var observers = container.Resolve<IObserverIOType[]>();

            bool success = false;
            foreach (var item in observers)
            {
                if (item.Unlock(guidValue))
                {
                    success = true;   
                    item.Send(guidValue);
                }
            }

            if (success == true)
                return;

            if (guidValue is { Value: GuidBase { Exception: Exception exception } })
            {
                return;
            }
            if (guidValue is { Value: GuidBase { IsComplete: true } })
            {
                return;
            }
            if (guidValue is { Value: GuidBase { Progress: Progress progress } })
            {
                return;
            }
        }

        public Interfaces.Generic.IObservable<TOutput> Register<TInput, TOutput>(Key baseKey, TInput tInput) where TInput : IGuid
        {
            var replay = new Subject<TOutput>();
            var key = new Key(Guid.NewGuid(), nameof(CustomSubject<TInput, TOutput>), typeof(CustomSubject<TInput, TOutput>));

            var subject = new CustomSubject<TInput, TOutput>(key, baseKey, a =>
            {
                if (a.Value is GuidBase guidBase)
                {
                    if (guidBase.IsComplete)
                    {
                        completedSubject.OnNext(key);
                        replay.OnCompleted();
                    }
                    else if (guidBase.Exception is Exception ex)
                    {
                        exceptionSubject.OnNext((key, ex));
                        replay.OnError(ex);
                    }
                    else if (guidBase.Progress is Progress progress)
                    {
                        progressSubject.OnNext((key, progress));
                        replay.OnProgress(progress.Amount, progress.Total);
                    }
                }
                else
                    replay.OnNext((TOutput)a.Value);
                return default;
            });

            var source = new GuidValue(tInput, new GuidValue(key)/*, new(guid, subject.Key)*/);

            //container.RegisterInstance<IObserverIOType>(subject);
            observers.Add(subject);

            //if (dictionary.ContainsKey(guid.Guid))
            //{
            //    throw new InvalidOperationException("ds es");
            //}
            //dictionary[SingleParameterMethod.Combine(guid.Guid, typeof(TOutput).GUID)] = subject;
            Logger.Add(subject).Wait();

            //baseObject.Output = source;

            factory.StartNew(() =>
            {
                Send(source);
            });

            return replay;
        }

        //public void Clear()
        //{
        //    foreach (var keyValuePair in dictionary)
        //    {

        //        if (keyValuePair.Value is SingleParameterMethod singleParameterMethod)
        //        {
        //            foreach (var disposable in singleParameterMethod.disposables)
        //                disposable.Dispose();
        //        }
        //        else
        //        {
        //            dictionary.Remove(keyValuePair.Key, out var observer);              
        //            if (observer is Subject subject)
        //            {
        //                subject.Outputs.Clear();
        //                subject.Observers.Clear();
        //            }
        //        }
        //    }
        //}

        //private void Add(IObserverIOType add)
        //{
        //    foreach (var observer in Observers)
        //    {
        //        observer.OnNext(new ChangeSet<IObserverIOType>(new Change<IObserverIOType>(add, ChangeType.Add)));
        //    }
        //}

        //private void Remove(IObserverIOType remove)
        //{
        //    foreach (var observer in Observers)
        //    {
        //        observer.OnNext(new ChangeSet<IObserverIOType>(new Change<IObserverIOType>(remove, ChangeType.Remove)));
        //    }
        //}

        //public List<IObserver<ChangeSet<IObserverIOType>>> Observers { get; } = new();

        //public IDisposable Subscribe(IObserver<ChangeSet<IObserverIOType>> observer)
        //{
        //    return new Disposer<ChangeSet<IObserverIOType>>(Observers, observer);
        //}
    }

    public class CustomSubject<TInput, TOutput> : IObserverIOType
    {

        private readonly Func<GuidValue, TOutput> onNext;

        public CustomSubject(Key key, Key parentKey, Func<GuidValue, TOutput> onNext)
        {
            Key = key;
            this.ParentKey = parentKey;
            this.onNext = onNext;
        }

        public Key ParentKey { get; set; }

        public Key Key { get; }

        public Type InType => typeof(TInput);

        public Type OutType => typeof(TOutput);

        public void Send(GuidValue guidValue)
        {
            onNext.Invoke(guidValue);
        }

        //ObservableCollection<object> IObserverIOType.Observers => throw new NotImplementedException();

        //ObservableCollection<object> IObserverIOType.Outputs => throw new NotImplementedException();

        public bool Unlock(GuidValue guidValue)
        {
            return guidValue.Target.Equals(typeof(TOutput).GUID) && guidValue.Source.Equals(Key.Guid);
        }

        public override string ToString()
        {
            return typeof(TInput).Name + " ~ " + typeof(TOutput).Name;
        }
    }
}
