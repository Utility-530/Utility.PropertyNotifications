using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.Interfaces.NonGeneric
{
    public interface IFactory
    {
        object Create(object config);
    }
}
