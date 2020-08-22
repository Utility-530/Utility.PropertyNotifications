using System.Collections.Generic;

namespace UtilityInterface.Generic
{
    public interface IParent<T>
    {
        IEnumerable<T> Children { get; set; }

    }

}
