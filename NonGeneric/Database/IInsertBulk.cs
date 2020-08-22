using System.Collections;

namespace UtilityInterface.NonGeneric.Database
{
    public interface IInsertBulk
    {
        int InsertBulk(IEnumerable item);
    }
}
