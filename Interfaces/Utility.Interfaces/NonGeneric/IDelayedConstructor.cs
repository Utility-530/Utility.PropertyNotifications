using System.Threading.Tasks;

namespace Utility.Interfaces.NonGeneric
{
    public interface IDelayedConstructor
    {
        Task<bool> Init(object o);
    }
}