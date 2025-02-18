using System.Collections.Generic;

namespace Utility.Nodes.Demo.MVVM
{
    public class Model
    {
        public List<Table> Tables { get; set; } = [];
        public Table SelectedTable { get; set; }
    }

    public class Models
    {
        public List<Model> Collection { get; set; } = [];

    }

}
