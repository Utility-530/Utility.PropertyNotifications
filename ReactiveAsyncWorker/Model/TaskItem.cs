using UtilityInterface.Generic;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveAsyncWorker.Model
{
    public class TaskItem<T> : IKey<string>
    {
        public TaskItem(string key, Task<T> task)
        {
            Key = key;
            Task = task;
        }

        public string Key { get; }

        public Task<T> Task { get; }
    }
}
