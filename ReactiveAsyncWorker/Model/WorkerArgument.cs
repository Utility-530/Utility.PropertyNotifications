using UtilityInterface.Generic;
using System;
using System.Collections.Generic;
using System.Text;

namespace ReactiveAsyncWorker
{
    public struct WorkerArgument<T> : IKey<string>
    {
        public int Iterations { get; set; }

        public long Delay { get; set; }

        public IMethodContainer<T> MethodContainer { get; set; }

        public TimeSpan Timeout { get; set; }

        public string Key { get; set; }
    }

}
