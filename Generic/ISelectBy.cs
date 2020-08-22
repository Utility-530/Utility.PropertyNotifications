using System;
using System.Collections.Generic;

namespace UtilityInterface.Generic.Database
{
    public interface ISelectBy<T>
    {
        IEnumerable<T> SelectBy(Func<T,bool> id);
    }

}
