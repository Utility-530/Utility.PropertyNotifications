using System.Reactive.Disposables;
using Utility.Nodes.Ex;
using Utility.Nodes.Filters;
using Utility.Nodes.WPF;
using Utility.Trees.Abstractions;

namespace Utility.Nodes.Demo.Transformers
{
    public class InputSelectionViewModel : ViewModel
    {
        public IEnumerable<IReadOnlyTree> Content =>  get(nameof(NodeMethodFactory.BuildInputNodeRoot));
        public IEnumerable<IReadOnlyTree> Controls => get(nameof(NodeMethodFactory.BuildInputControlRoot));

    }
}


