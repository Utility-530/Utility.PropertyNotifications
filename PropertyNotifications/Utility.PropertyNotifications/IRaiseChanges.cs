using System.Runtime.CompilerServices;
using Utility.Interfaces;

namespace Utility.PropertyNotifications
{
    public interface IRaiseChanges : IRaisePropertyChanged, IRaiseExPropertyChanged
    {
        bool RaisePropertyCalled(object? value, [CallerMemberName] string? propertyName = null);

        bool RaisePropertyReceived(object value, object oldValue, [CallerMemberName] string? propertyName = null);
    }
}