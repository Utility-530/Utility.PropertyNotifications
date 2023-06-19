using System;
using Utility.Interfaces.Generic;

namespace Utility.Tasks
{
    public interface IWorkerArgument<T>: IKey<string>
    {
        long Delay { get; set; }

        int Iterations { get; set; }
        
        IMethodContainer<T> MethodContainer { get; set; }

        TimeSpan Timeout { get; set; }
    }
}