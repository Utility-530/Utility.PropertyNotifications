using Utility.Tasks.Model;
using System;
using System.Reactive.Concurrency;
using System.Threading.Tasks;

namespace Utility.Tasks.ViewModel
{
    //public class TPLStringQueue : AsyncWorkerBaseQueue<TaskItem<string>>
    //{
    //    public TPLStringQueue(IScheduler scheduler) : base(scheduler)
    //    {
    //    }

    //    protected override IObservable<ProgressState> Convert(IObservable<TaskItem<string>> newitems)
    //    {
    //        var cts = new System.Threading.CancellationTokenSource();
    //        var scheduler = new SynchronizationContextScheduler(System.Threading.SynchronizationContext.Current);
    //        var queue = new DataFlowQueue<string>(cts, scheduler);
    //        newitems.Subscribe(queue.OnNext);
    //        return queue;
    //    }
    //}
}
