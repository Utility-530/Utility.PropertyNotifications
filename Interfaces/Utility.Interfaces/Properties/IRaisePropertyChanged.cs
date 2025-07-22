
using System.Runtime.CompilerServices;

namespace Utility.Interfaces
{
    public interface IRaisePropertyChanged
    {
        public bool RaisePropertyChanged([CallerMemberName] string? propertyName = null);

    }

    public interface IRaiseExPropertyChanged
    {
        public bool RaisePropertyChanged<T>(ref T previousValue, T value, [CallerMemberName] string? propertyName = null);

    }
}
