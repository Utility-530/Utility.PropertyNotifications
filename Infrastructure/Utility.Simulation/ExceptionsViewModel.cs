using System.Collections.ObjectModel;
using Utility.PropertyNotifications;

namespace Utility.Simulation
{
    public class ExceptionsViewModel : NotifyPropertyClass
    {
        public int GridRow => 2;
        public ObservableCollection<object> Collection { get; } = [];
    }
}