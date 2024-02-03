using Utility.PropertyNotifications;
using System.ComponentModel;

namespace Utility.PropertyNotifications
{
    public interface INotifyProperty : INotifyPropertyChanged, INotifyPropertyCalled, INotifyPropertyReceived
    {
    }
}