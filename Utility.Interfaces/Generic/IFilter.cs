using System.Collections.Generic;

namespace Utility.Interfaces.Generic
{
    public interface IFilter<T>
    {
        IEnumerable<T> Filter(IEnumerable<T> o);
    }
}