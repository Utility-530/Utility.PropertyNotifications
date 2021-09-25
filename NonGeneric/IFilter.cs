using System.Collections;

namespace UtilityInterface.NonGeneric
{
    public interface IFilter
    {
        IEnumerable Filter(IEnumerable o);

    }
}
