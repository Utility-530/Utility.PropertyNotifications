using System;
using System.Collections.Generic;
using System.Text;

namespace UtilityInterface.Generic.Database
{
    public interface ISetId<T>
    {
        T Id { set; }
    }
}
