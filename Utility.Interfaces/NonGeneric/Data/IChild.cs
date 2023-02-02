using System.Collections.Generic;
using System.Text;

namespace Utility.Interfaces.NonGeneric.Data
{

    public interface IChild : IId
    {
        long ParentId { get; }
    }
}
