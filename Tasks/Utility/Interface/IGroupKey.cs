using System;
using System.Collections.Generic;
using System.Text;

namespace Utility
{
    public interface IGroupKey<TType>
    {
        public TType GroupKey { get; }

    }
}
