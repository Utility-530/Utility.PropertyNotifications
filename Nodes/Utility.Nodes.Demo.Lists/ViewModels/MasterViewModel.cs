using Splat;
using System.Reactive.Disposables;
using Utility.Nodes.Filters;
using Utility.Nodes.WPF;
using Utility.Trees.Abstractions;

namespace Utility.Nodes.Demo.Lists
{
    public class MasterViewModel : ViewModel
    {
        IReadOnlyTree[] selection, control;


        public IReadOnlyTree[] Selection
        {
            get
            {
                if (selection == null)
                    Locator.Current.GetService<MethodCache>().Get(nameof(NodeMethodFactory.BuildListRoot))
                        .Subscribe(a => 
                        {
                            selection = [a]; RaisePropertyChanged(nameof(Selection));
                        })
                        .DisposeWith(disposables);
                return selection;
            }
        }

        public IReadOnlyTree[] Controls
        {
            get
            {
                if (control == null)
                    Locator.Current.GetService<MethodCache>().Get(nameof(NodeMethodFactory.BuildControlRoot))
                        .Subscribe(a =>
                        {
                            control = [a]; base.RaisePropertyChanged(nameof(Controls));
                        }).DisposeWith(disposables);
                return control;
            }
        }
    }


}
