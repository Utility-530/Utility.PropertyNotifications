using ReactiveAsyncWorker.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveAsyncWorker.ViewModel.Service
{
    public class TaskItemFactory<T> : IFactory<IWorkerItem<T>, TaskItem<T>>
    {
        public IWorkerItem<T> Create(TaskItem<T> args)
        {
            return new AsyncWorkerItem<T>(args.Key, args.Task);
        }
    }

    public class StringTaskItemFactory : TaskItemFactory<StringTaskOutput>
    {
    }
}
