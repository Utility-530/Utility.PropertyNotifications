using Splat;
using Utility.Nodes.Meta;
using Utility.Nodes.WPF;
using Utility.Observables.Generic;
using Utility.Trees.Abstractions;

namespace Utility.Nodes.Demo.Lists
{
    public class TreeViewModel(string path, Guid? guid = default, params object?[]? objects) : ViewModel
    {
        IReadOnlyTree[]? selection;

        public IReadOnlyTree[] Nodes
        {
            get
            {
                if (selection == null)
                    Locator.Current.GetService<MethodCache>().Get(path, guid, objects)
                        .Subscribe(a => { selection = [a]; RaisePropertyChanged(nameof(Nodes)); }).DisposeWith(disposables);
                return selection;
            }
        }

    }
}
