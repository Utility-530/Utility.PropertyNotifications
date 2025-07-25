using System.Collections.ObjectModel;
using Utility.Interfaces.Exs;
using Utility.PropertyNotifications;

namespace Utility.Simulation
{ 

    public class HistoryViewModel : NotifyPropertyClass
    {
        private int index = -1;

        public int GridRow => 1;

        public int Index { get => index; set => this.RaisePropertyChanged(ref index, value); }
        public object CurrentItem => Collection[Index];

        public ObservableCollection<object> Collection { get; } = [];

    }
}
