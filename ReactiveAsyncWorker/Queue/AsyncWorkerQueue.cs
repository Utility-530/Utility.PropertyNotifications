using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Reactive.Linq;
using UtilityInterface.Generic;
using UtilityEnum;
using ReactiveAsyncWorker.Model;

namespace ReactiveAsyncWorker
{

    public abstract class AsyncWorkerQueue<TOutput> : AsyncWorkerQueue
    {       
        private readonly Queue<IWorkerItem<TOutput>> queue = new Queue<IWorkerItem<TOutput>>();
      
            public AsyncWorkerQueue(IObservable<TOutput> obs)
        {
                obs
                .Subscribe(a =>
                {
                    var s = queue.TryDequeue(out var output);

                    if (!s)
                    {
                        return;
                    }
                    output.Start();

                    lock (queue)
                    {
                        if (queue.Count > 0)
                            (queue.Peek()).Start();
                    }
                });
        }

        public void Enqueue(IWorkerItem<TOutput> qitem)
        {
            queue.Enqueue(qitem);

            qitem.Subscribe(subject.OnNext);

            lock (queue)
            {
                if (queue.Count > 0)
                    (queue.Peek()).Start();
            }
        }


    }
    public abstract class AsyncWorkerQueue : IObservable<ProgressState> , IObserver<ProcessState>
    {
        protected readonly ISubject<ProcessState> commands = new Subject<ProcessState>();
        protected readonly ISubject<ProgressState> subject = new ReplaySubject<ProgressState>();

        public AsyncWorkerQueue()
        {
         
        }

        public IDisposable Subscribe(IObserver<ProgressState> observer)
        {
            return subject.Subscribe(observer);
        }

        public void OnNext(ProcessState value)
        {
            commands.OnNext(value);
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }
    }
}



