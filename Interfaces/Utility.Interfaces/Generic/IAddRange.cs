using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.Interfaces.Generic
{
    public interface IAddRange<T>
    {
        void AddRange(IEnumerable<T> collection);
    }
}
