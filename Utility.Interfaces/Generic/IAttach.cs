using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.Interfaces.Generic
{
    public interface IAttach<T>
    {
        void Attach(T value);
    }
}
