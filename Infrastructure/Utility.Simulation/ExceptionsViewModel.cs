using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.PropertyNotifications;

namespace Utility.Simulation
{
    public class ExceptionsViewModel: NotifyPropertyClass
    {
        public int GridRow => 2;
        public ObservableCollection<object> Collection { get; } = [];
    }
}
