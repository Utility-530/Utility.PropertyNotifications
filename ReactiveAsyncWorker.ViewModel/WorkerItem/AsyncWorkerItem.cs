using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveAsyncWorker
{
    public class AsyncWorkerItem<T> : WorkerItem<T> 
    {
        private readonly Task<T> task;

        public AsyncWorkerItem(string key,Task<T> task) : base(key, task.ToObservable())
        {
            this.task = task;
        }
        public override void Start()
        {
            task.Start();
            base.Start();
        }
    }

}
