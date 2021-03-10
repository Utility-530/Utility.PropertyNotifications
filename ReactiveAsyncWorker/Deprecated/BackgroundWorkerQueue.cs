using ReactiveAsyncWorker;
using ReactiveAsyncWorker.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace ReactiveAsyncWorker.ViewModel
{
    //public class BackgroundWorkerQueue : AsyncWorkerBaseQueue<WorkerArgument<object>>
    //{
    //    public BackgroundWorkerQueue(IScheduler scheduler) : base(scheduler)
    //    {
    //    }

    //    protected override IObservable<ProgressState> Convert(IObservable<WorkerArgument<object>> newitems)
    //    {
    //        return new BackgroundWorkerCommandQueue<object>(newitems);
    //    }
    //}
}
