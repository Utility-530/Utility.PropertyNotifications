using System.Collections;

namespace Utility.Interfaces.NonGeneric
{
    public interface IFilter
    {
        IEnumerable Filter(IEnumerable o);

    }
}
