using System.Reactive.Disposables;
using Utility.Nodes.Meta;
using Utility.Trees.Abstractions;

namespace Utility.Nodes.Demo.Editor
{
    public class MasterViewModel : ViewModel
    {
        IReadOnlyTree[] selection, control;

        public IReadOnlyTree[] Selection
        {
            get
            {
                if (selection == null)
                    Subscribe(nameof(NodeMethodFactory.BuildComboRoot), a => selection = [a]);
                return selection;
            }
        }

        public IReadOnlyTree[] Controls
        {
            get
            {
                if (control == null)
                    Subscribe(nameof(NodeMethodFactory.BuildControlRoot), a => control = [a]);       
                return control;
            }
        }
    }
}
