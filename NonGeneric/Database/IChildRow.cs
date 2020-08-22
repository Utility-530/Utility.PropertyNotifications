using System.Collections.Generic;
using System.Text;

namespace UtilityInterface.NonGeneric.Database
{



    public interface IChildRow : IId
    {
        long ParentId { get; }
    }
}
