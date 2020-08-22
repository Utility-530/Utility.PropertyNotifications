using System.Collections.Generic;

namespace UtilityInterface.Generic.Database
{
    public interface IInsertBulk<T>
    {
        int InsertBulk(IEnumerable<T> item);
    }

}
