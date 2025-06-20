using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Interfaces.Generic
{
    public interface ITaskFactory<T>
    {
        Task<T> Create(object config);
    }

    public interface IFactory<T>
    {
        T Create(object config);
    }

    public interface IEnumerableFactory<T>
    {
        IEnumerable<T> Create(object config);
    }
}
