using System.Reactive.Disposables;
using Utility.Nodes.Ex;
using Utility.Nodes.Filters;
using Utility.Nodes.WPF;
using Utility.Trees.Abstractions;

namespace Utility.Nodes.Demo.Transformers
{
    public class MainViewModel : ViewModel
    {     

        public IEnumerable<IReadOnlyTree> Content => get(nameof(Factory.BuildDemoContentRoot));
        public IEnumerable<IReadOnlyTree> Controls => get(nameof(Factory.BuildControlRoot));
        public IEnumerable<IReadOnlyTree> Transformers => get(nameof(Factory.BuildTransformersRoot));

    }
}


