using System.Collections.Generic;

namespace UtilityInterface.Generic.Database
{
    public interface IUpdateBulk<T>
    {
        int UpdateBulk(IEnumerable<T> item);
    }

}
