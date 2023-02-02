using System;
using System.Collections.Generic;
using System.Text;
using Utility.Interfaces.NonGeneric.Data;

namespace Utility.Interfaces.Generic.Data
{
    public interface IEntity<T>: IEntity
    {
        T Object { get; set; }

    }
}
