using Splat;
using Utility.Interfaces.Exs;
using Utility.Interfaces.NonGeneric;
using Utility.Nodes.Filters;
using Utility.PropertyNotifications;
using Utility.Trees;
using Utility.Trees.Abstractions;

namespace Utility.Nodes.Demo.Directory
{
    public class MainViewModel : NotifyPropertyClass
    {
        Lazy<INodeSource> source = new(() => Locator.Current.GetService<INodeSource>());

        private ITree selection;

        public ITree Selection
        {
            get
            {
                if (selection == null)
                    source.Value.Single(nameof(Factory.BuildComboRoot))
                        .Subscribe(a => { selection = a;
                            RaisePropertyChanged(nameof(Selection)); });
                return selection ?? new Tree();
            }
        }

    }
}
