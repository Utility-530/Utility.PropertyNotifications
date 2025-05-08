using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Interfaces.Generic
{
    public interface IAdd<T>
    {
        void Add(T item);
    }
}
