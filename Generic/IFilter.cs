using System.Collections.Generic;

namespace UtilityInterface.Generic
{
    public interface IFilter<T>
    {
        IEnumerable<T> Filter(IEnumerable<T> o);

    }

}
