using System;
using System.Collections.Generic;
using System.Text;

namespace ReactiveAsyncWorker.Interface
{
    public interface IGroupKey<TType>
    {
        public TType GroupKey { get; }

    }
}
