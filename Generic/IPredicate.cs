using System;
using System.Collections.Generic;
using System.Text;

namespace UtilityInterface.Generic
{

    public interface IPredicate<T>
    {
        bool Check(T value);
    }

}
