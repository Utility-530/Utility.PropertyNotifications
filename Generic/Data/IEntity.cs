using System;
using System.Collections.Generic;
using System.Text;
using UtilityInterface.NonGeneric.Data;

namespace UtilityInterface.Generic.Data
{
    public interface IEntity<T>: IEntity
    {
        T Object { get; set; }

    }
}
