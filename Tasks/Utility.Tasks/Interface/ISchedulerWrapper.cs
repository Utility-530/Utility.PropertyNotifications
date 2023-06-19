using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    public interface ISchedulerWrapper
    {
        public IScheduler Scheduler { get; }
    }
}
