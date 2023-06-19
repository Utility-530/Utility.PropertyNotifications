using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using Utility.Infrastructure;
using Utility.Tasks.Model;
using Utility.Enums;
using Utility.Interfaces.NonGeneric;
using Utility.Progressions;

namespace Utility.Tasks
{
    public class AsyncWorkerItem : IWorkerItem
    {
        private readonly Task<ITaskOutput> task;
        private readonly CancellationTokenSource tokenSource;
        private readonly ReplaySubject<IProgressState> replaySubject = new();
        private readonly ReplaySubject<Unit> startSubject = new();

        public AsyncWorkerItem(string key,
            IObservable<TaskChangeRequest> taskChangeRequest,
            Task<ITaskOutput> task,
            TimeSpan estimatedTimeSpan,
          
            CancellationTokenSource tokenSource,
              bool startAsSoonAsPossible = true)
        {
            taskChangeRequest.Select(a => a)
                .Where(a => a.Key == key)
                .Subscribe(a =>
                {
                    var action = a.State switch
                    {
                        RunningState.None => new Action(() => { }),
                        RunningState.Pause => Pause,
                        RunningState.Play => Start,
                        RunningState.Stop => Stop,
                        _ => throw new NotImplementedException(),
                    };
                    action();
                });

            Key = key;
            this.task = task;
            this.tokenSource = tokenSource;




            CreateNotifications(key, task, startSubject, estimatedTimeSpan, tokenSource.Token, startAsSoonAsPossible)
                .Subscribe(replaySubject);
        }

        public string Key { get; }

        static IObservable<IProgressState> CreateNotifications(string key,
            Task<ITaskOutput> task, IObservable<Unit> startSubject, TimeSpan timeSpan, CancellationToken token,
            bool startAsSoonAsPossible)
        {
            return Observable.Create<ProgressState>(changes =>
            {
                var started = DateTime.Now;

                changes.OnNext(new ProgressState(key, 
                    startAsSoonAsPossible ? ProcessState.Ready : ProcessState.Created, 
                    new ProgressItem(0, false, false, timeSpan, started), 
                    default));

                token.Register(() =>
                {
                    changes.OnNext(new ProgressState(
                             key,
                             ProcessState.Blocked,
                             new ProgressItem(0.5, true, false, default, started),
                             default));

                });

                var endTask = task
                 .ToObservable()
                 .Take(1)
                 .Select(state =>
                 {
                     if (state.IsCancelled)
                     {
                         var progressStateCancelled = new ProgressState(
                             key,
                             ProcessState.Created,
                             new ProgressItem(0, false, false, default, default),
                             state);

                         return progressStateCancelled;
                     }

                     if (task.IsCompletedSuccessfully == false)
                     {
                         var progressStateBlocked = new ProgressState(
                         key,
                         ProcessState.Blocked,
                         new ProgressItem(0.5, true, false, default, started),
                         state);
                         return progressStateBlocked;
                     }

                     var progressState = new ProgressState(
                            key,
                            ProcessState.Terminated,
                            new ProgressItem(1.0, true, false, default, started),
                            state);
                     return progressState;
                 });


                return startSubject
                    .Select(a => new ProgressState(key, ProcessState.Running, new ProgressItem(0.5, false, true, timeSpan, started), default))
                    .Merge(endTask)

                        .Subscribe(pState =>
                        {
                            if (token.IsCancellationRequested)
                            {


                            }
                            changes.OnNext(pState);
                            var d = task.Status;
                            var e = task.IsCanceled;
                            if (e)
                            {

                            }
                        }, e =>
                        {
                        }, () =>
                        {
                        });
            });
        }

        public bool Equals(Interfaces.Generic.IKey<string> other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(IEquatable other)
        {
            throw new NotImplementedException();
        }

        public void Pause()
        {
            //tokenSource.Cancel();
            throw new NotImplementedException();
        }

        public void Start()
        {
            startSubject.OnNext(Unit.Default);
            task.Start();
        }

        public void Stop()
        {
            tokenSource.Cancel(false);
        }

        public IDisposable Subscribe(IObserver<IProgressState> observer)
        {
            return replaySubject.Subscribe(observer);
        }
    }
}
