using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Interfaces.Generic
{
    public interface IFactory<T>
    {
        Task<T> Create(object config);
    }
}
