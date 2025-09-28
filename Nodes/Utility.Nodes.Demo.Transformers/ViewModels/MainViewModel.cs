using System.Reactive.Disposables;
using Utility.Nodes.Ex;
using Utility.Nodes.Meta;
using Utility.Nodes.WPF;
using Utility.Trees.Abstractions;

namespace Utility.Nodes.Demo.Transformers
{
    public class MainViewModel : ViewModel
    {     

        public IEnumerable<IReadOnlyTree> Content => get(nameof(NodeMethodFactory.BuildDemoContentRoot));
        public IEnumerable<IReadOnlyTree> Controls => get(nameof(NodeMethodFactory.BuildControlRoot));
        public IEnumerable<IReadOnlyTree> Transformers => get(nameof(NodeMethodFactory.BuildTransformersRoot));

    }
}


