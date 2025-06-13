using Splat;
using System.Reactive.Disposables;
using Utility.Nodes.Ex;
using Utility.Nodes.Filters;
using Utility.Nodes.WPF;
using Utility.Trees.Abstractions;

namespace Utility.Nodes.Demo.Lists
{
    public class SlaveViewModel : ViewModel
    {
        IReadOnlyTree[] content;

        public IReadOnlyTree[] Content
        {
            get
            {
                if (content == null)
                    Locator.Current.GetService<MethodCache>().Get(nameof(NodeMethodFactory.BuildContentRoot))
                        .Subscribe(a => { content = [a]; RaisePropertyChanged(nameof(Content)); })
                        .DisposeWith(disposables); ;
                return content;
            }
        }        
    }
}


