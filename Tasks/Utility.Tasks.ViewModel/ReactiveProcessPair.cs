using System.Collections.Generic;
using Utility.Enums;
using Utility.Progressions;

namespace Utility.Tasks.ViewModel
{
    public class ReactiveProcessPair : ReactivePair<ProcessState, IEnumerable<IProgressState>>
    {
        public ReactiveProcessPair(ProcessState key, IEnumerable<IProgressState> value) : base(key, value)
        {
        }

        public static new ReactiveProcessPair Create(ProcessState key, IEnumerable<IProgressState> value)
        {
            return new ReactiveProcessPair(key, value);
        }
    }
}
