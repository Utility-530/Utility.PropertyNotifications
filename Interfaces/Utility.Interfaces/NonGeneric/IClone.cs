using System.Threading.Tasks;

namespace Utility.Interfaces.NonGeneric
{
    public interface IClone
    {
        object Clone();
    }

    public interface IAsyncClone
    {
        Task<object> AsyncClone();
    }
}