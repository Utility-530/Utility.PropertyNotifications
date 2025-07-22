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
                    Subscribe(nameof(NodeMethodFactory.BuildListRoot),
                        a =>
                        {
                            selection = [a];
                        });
                return selection;
            }
        }

        //public IReadOnlyTree[] Controls
        //{
        //    get
        //    {
        //        if (control == null)
        //            Subscribe(nameof(NodeMethodFactory.BuildControlRoot), a =>
        //                {
        //                    control = [a]; base.RaisePropertyChanged(nameof(Controls));
        //                });
        //        return control;
        //    }
        //}
    }


}
