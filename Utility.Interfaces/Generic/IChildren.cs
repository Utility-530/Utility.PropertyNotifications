using System.Collections.Generic;

namespace Utility.Interfaces.Generic
{
    public interface IChildren<T>
    {
        IEnumerable<T> Children { get; set; }
    }
}