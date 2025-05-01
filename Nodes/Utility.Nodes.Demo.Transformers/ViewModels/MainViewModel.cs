using System.Reactive.Disposables;
using Utility.Nodes.Ex;
using Utility.Nodes.Filters;
using Utility.Nodes.WPF;
using Utility.Trees.Abstractions;

namespace Utility.Nodes.Demo.Transformers
{
    public class MainViewModel : ViewModel
    {     

        public IReadOnlyTree[] Content => get(nameof(Factory.BuildDemoContentRoot), nameof(Content));
        public IReadOnlyTree[] Controls => get(nameof(Factory.BuildControlRoot), nameof(Controls));
        public IReadOnlyTree[] Transformers => get(nameof(Factory.BuildTransformersRoot), nameof(Transformers));

    }
}


