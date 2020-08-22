using System.Collections;

namespace UtilityInterface.NonGeneric.Database
{
    public interface IDeleteBulk
    {
        int DeleteBulk(IEnumerable item);
    }
}
