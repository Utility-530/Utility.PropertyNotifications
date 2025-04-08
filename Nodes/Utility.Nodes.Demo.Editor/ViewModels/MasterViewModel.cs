using System.Reactive.Disposables;
using Utility.Nodes.Filters;
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
                    source.Value.Single(nameof(Factory.BuildComboRoot))
                        .Subscribe(a => { selection = [a]; RaisePropertyChanged(nameof(Selection)); }).DisposeWith(disposables);
                return selection;
            }
        }

        public IReadOnlyTree[] Controls
        {
            get
            {
                if (control == null)
                    source.Value.Single(nameof(Factory.BuildControlRoot))
                        .Subscribe(a =>
                        {
                            control = [a]; base.RaisePropertyChanged(nameof(Controls));
                        }).DisposeWith(disposables);
                return control;
            }
        }
    }
}
