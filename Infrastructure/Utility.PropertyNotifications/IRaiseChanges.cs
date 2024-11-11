using System.Runtime.CompilerServices;

namespace Utility.PropertyNotifications
{
    public interface IRaiseChanges
    {
        void RaisePropertyCalled(object? value, [CallerMemberName] string? propertyName = null);
        void RaisePropertyChanged([CallerMemberName] string? propertyName = null);
        void RaisePropertyReceived(object value, [CallerMemberName] string? propertyName = null);
    }
}