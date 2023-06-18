using UtilityInterface.Generic;
using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.Tasks
{
    public struct WorkerArgument<T> : IWorkerArgument<T>
    {
        public int Iterations { get; set; }

        public long Delay { get; set; }

        public IMethodContainer<T> MethodContainer { get; set; }

        public TimeSpan Timeout { get; set; }

        public string Key { get; set; }
    }

}
