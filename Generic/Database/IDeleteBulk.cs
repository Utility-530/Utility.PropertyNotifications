using System.Collections.Generic;

namespace UtilityInterface.Generic.Database
{
    public interface IDeleteBulk<T>
    {
        int DeleteBulk(IEnumerable<T> item);
    }

}
