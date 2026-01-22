using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Utility.Interfaces.NonGeneric
{
    public interface IFactory
    {
        object Create(object config);
    }
    public interface ITaskFactory
    {
        Task<object> Create(object config);
    }

    public interface IEnumerableFactory
    {
        IEnumerable Create(object config);
    }
}