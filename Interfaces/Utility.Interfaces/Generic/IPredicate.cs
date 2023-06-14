using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.Interfaces.Generic
{

    public interface IPredicate<T>
    {
        bool Check(T value);
    }

}
