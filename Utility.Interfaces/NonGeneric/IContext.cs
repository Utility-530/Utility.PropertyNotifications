using System.Threading;

namespace Utility.Interfaces.NonGeneric
{
    public interface IContext
    {
        SynchronizationContext UI { get; }
    }
}