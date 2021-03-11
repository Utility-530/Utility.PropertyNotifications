using System.Collections.Generic;
using UtilityEnum;
using ReactiveAsyncWorker.Model;

namespace ReactiveAsyncWorker.ViewModel
{
    public class ReactiveProcessPair : ReactivePair<ProcessState, IEnumerable<ProgressState>>
    {
        public ReactiveProcessPair(ProcessState key, IEnumerable<ProgressState> value) : base(key, value)
        {
        }

        public static new ReactiveProcessPair Create(ProcessState key, IEnumerable<ProgressState> value)
        {
            return new ReactiveProcessPair(key, value);
        }
    }
}
