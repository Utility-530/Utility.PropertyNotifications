using System.Collections.ObjectModel;

namespace Utility.Nodes.Demo
{
    public class MasterModel
    {
        public ObservableCollection<Master> Collection { get; set; } = new();
    }
}

