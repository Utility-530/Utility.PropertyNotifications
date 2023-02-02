using System.Threading.Tasks;

namespace Utility.Interface.NonGeneric
{
    public interface IDelayedConstructor
    {
        Task<bool> Init(object o);
    }
}