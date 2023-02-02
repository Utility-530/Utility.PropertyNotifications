using System.Collections.Generic;

namespace Utility.Interfaces.Generic
{
    public interface IParent<T>
    {
        IEnumerable<T> Children { get; set; }

    }

}
