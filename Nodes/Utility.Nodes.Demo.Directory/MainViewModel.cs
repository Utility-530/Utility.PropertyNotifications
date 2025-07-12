using Splat;
using Utility.Interfaces.Exs;
using Utility.Interfaces.NonGeneric;
using Utility.Nodes.Filters;
using Utility.Nodes.WPF;
using Utility.PropertyNotifications;
using Utility.Trees;
using Utility.Trees.Abstractions;

namespace Utility.Nodes.Demo.Directory
{
    public class MainViewModel : ViewModel
    {

        private IReadOnlyTree selection;

        public IReadOnlyTree Selection
        {
            get
            {
                if (selection == null)
                    Subscribe(nameof(NodeMethodFactory.BuildComboRoot), a => selection = a);
                return selection ?? new Tree();
            }
        }

    }
}
