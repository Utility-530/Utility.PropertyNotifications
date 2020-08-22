using System.Collections.Generic;

namespace UtilityInterface.Generic.Database
{
    public interface ISelectAll<T>
    {
        IEnumerable<T> SelectAll();
    }

}
