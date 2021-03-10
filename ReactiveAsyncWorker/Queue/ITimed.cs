
using System;

namespace ReactiveAsyncWorker
{
    public interface ITimed
    {
        public DateTime Start { get; }

        public DateTime Finish { get; }
    }
}


