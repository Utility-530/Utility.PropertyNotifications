using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Text;

namespace ReactiveAsyncWorker
{
    public interface ISchedulerWrapper
    {
        public IScheduler Scheduler { get; }
    }
}
