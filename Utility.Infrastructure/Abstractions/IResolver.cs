using Utility.Interfaces.NonGeneric;
using Utility.Models;

namespace Utility.Infrastructure.Abstractions
{
    public interface IResolver
    {
        public ICollection<IObserver> Observers(IEquatable key);
    }
}