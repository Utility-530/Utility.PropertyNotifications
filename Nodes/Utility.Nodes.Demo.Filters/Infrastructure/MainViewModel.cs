using Utility.Nodes.Filters;
using Utility.Trees.Abstractions;
using Utility.ViewModels;

namespace Utility.Nodes.Demo.Filters
{
    public class MainViewModel :NotifyPropertyChangedBase
    {
        IReadOnlyTree[] control, selection, content, filters;

        public IReadOnlyTree[] Controls
        {
            get
            {
                if (control != null)
                    return control;
                NodeSource
                    .Instance
                    .Single(nameof(Factory.BuildControlRoot))
                    .Subscribe(a =>
                    {
                        control = [a];
                        base.RaisePropertyChanged(nameof(Controls));
                    });

                return control;
            }
        }


        public IReadOnlyTree[] Selection
        {
            get
            {
                if (selection == null)
                    NodeSource.Instance.Single(nameof(Factory.BuildComboRoot))
                        .Subscribe(a => { selection = [a]; RaisePropertyChanged(nameof(Selection)); });
                return selection;
            }
        }

        public IReadOnlyTree[] Content
        {
            get
            {
                if (content == null)
                    NodeSource.Instance.Single(nameof(Factory.BuildContentRoot))
                        .Subscribe(a => { content = [a]; RaisePropertyChanged(nameof(Content)); });
                return content;
            }
        }

        public IReadOnlyTree[] Filters
        {
            get
            {
                if (filters == null)
                    NodeSource.Instance.Single(nameof(Factory.BuildFiltersRoot))
                        .Subscribe(a => { filters = [a]; RaisePropertyChanged(nameof(Filters)); });
                return filters;
            }
        }

        //public IReadOnlyTree BuildFiltersRoot
        //{
        //    get
        //    {
        //        if (content == null)
        //            NodeSource.Instance.Single(nameof(Factory.BuildDefault2))
        //                .Subscribe(a => { content = a; OnPropertyChanged(nameof(Content)); });
        //        return content;
        //    }
        //}
    }
}


