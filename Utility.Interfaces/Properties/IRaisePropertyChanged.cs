
using System.Runtime.CompilerServices;

namespace Utility.Interfaces
{
    public interface IRaisePropertyChanged
    {
        public void RaisePropertyChanged([CallerMemberName] string? propertyName = null);

    }

    public interface IRaiseExPropertyChanged
    {
        public void RaisePropertyChanged<T>(ref T previousValue, T value, [CallerMemberName] string? propertyName = null);

    }
}
