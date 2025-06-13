using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Models.Trees;
using Utility.Nodes.WPF;

namespace Utility.Nodes.Demo.Lists
{
    public class ChildViewModel(string name) : ViewModel
    {
        public override string Name => name;

        public ModelTypeModel Data { get; set; }
    }
}
