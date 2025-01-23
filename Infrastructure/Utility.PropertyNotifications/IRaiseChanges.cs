using System.Runtime.CompilerServices;
using Utility.Interfaces;

namespace Utility.PropertyNotifications
{
    public interface IRaiseChanges : IRaisePropertyChanged, IRaiseExPropertyChanged
    {
        void RaisePropertyCalled(object? value, [CallerMemberName] string? propertyName = null);
        void RaisePropertyReceived(object value, [CallerMemberName] string? propertyName = null);
    }
}