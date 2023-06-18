using System.Collections.Generic;
using UtilityEnum;
using Utility.Tasks.Model;

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
