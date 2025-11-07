using System.Collections.Generic;

namespace Utility.Interfaces.Generic
{
    public interface IAddRange<T>
    {
        void AddRange(IEnumerable<T> collection);
    }
}