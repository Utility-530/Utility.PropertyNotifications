using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Reactive.Linq;
using Utility.Enums;
using Utility.Progressions;

namespace Utility.Tasks
{


    public abstract class AsyncWorkerQueue : IObservable<IProgressState> , IObserver<ProcessState>
    {
        protected readonly Subject<ProcessState> commands = new();
        protected readonly ReplaySubject<IProgressState> subject = new();
        private readonly Queue<IWorkerItem> queue = new();

        public AsyncWorkerQueue(IObservable<ITaskOutput> obs)
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
                            queue.Peek().Start();
                    }
                });
        }

        public void Enqueue(IWorkerItem qitem)
        {
            queue.Enqueue(qitem);

            qitem.Subscribe(subject.OnNext);

            lock (queue)
            {
                if (queue.Count > 0)
                    queue.Peek().Start();
            }
        }

        public IDisposable Subscribe(IObserver<IProgressState> observer)
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

    //public abstract class AsyncWorkerQueue<TOutput> : AsyncWorkerQueue
    //{       
    //    private readonly Queue<IWorkerItem<TOutput>> queue = new Queue<IWorkerItem<TOutput>>();

    //        public AsyncWorkerQueue(IObservable<TOutput> obs)
    //    {
    //            obs
    //            .Subscribe(a =>
    //            {
    //                var s = queue.TryDequeue(out var output);

    //                if (!s)
    //                {
    //                    return;
    //                }
    //                output.Start();

    //                lock (queue)
    //                {
    //                    if (queue.Count > 0)
    //                        (queue.Peek()).Start();
    //                }
    //            });
    //    }

    //    public void Enqueue(IWorkerItem<TOutput> qitem)
    //    {
    //        queue.Enqueue(qitem);

    //        qitem.Subscribe(subject.OnNext);

    //        lock (queue)
    //        {
    //            if (queue.Count > 0)
    //                (queue.Peek()).Start();
    //        }
    //    }
    //}


}



