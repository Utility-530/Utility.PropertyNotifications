using System;
using System.Collections.Generic;
using System.Text;

namespace UtilityInterface.NonGeneric
{
    public interface IPredicate
    {
        bool Check(object value);
    }
}
