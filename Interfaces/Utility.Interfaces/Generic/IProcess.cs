using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.Interfaces.Generic
{
    public interface IProcess<T>
    {
        void Process(T value);
    }
}
