using System.Collections.Generic;
using System.Text;

namespace UtilityInterface.NonGeneric.Data
{

    public interface IChild : IId
    {
        long ParentId { get; }
    }
}
