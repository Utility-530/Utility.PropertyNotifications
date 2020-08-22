using System.Collections;

namespace UtilityInterface.NonGeneric.Database
{
    public interface IUpdateBulk
    {
        int UpdateBulk(IEnumerable item);
    }
}
