using DryIoc;
using Utility.Nodes;
using static Utility.PropertyTrees.WPF.Demo.Views.PropertyView;
using Utility.Observables.NonGeneric;
using Utility.Observables.Generic;

namespace Utility.PropertyTrees.WPF.Demo.Views
{
    /// <summary>
    /// Interaction logic for ViewModelView.xaml
    /// </summary>
    public partial class ViewModelView : UserControl
    {
        private ViewModelEngine viewModelEngine => container.Resolve<ViewModelEngine>();
        private ModelController controller => container.Resolve<ModelController>();
        private ValueNode masterNode => container.Resolve<ValueNode>(Keys.Model);

        public ViewModelView()
        {
            InitializeComponent();
            initialise_click(default, default);
        }

        private void initialise_click(object sender, RoutedEventArgs e)
        {
            //this.PropertyTree.SelectedObject = viewModel.Model;
            controller.TreeView(masterNode, PropertyTree)
                .Subscribe(treeView =>
            {
                //PropertyTree.Children.Clear();
                //PropertyTree.Children.Add(treeView);
            });
        }

        private void PropertyTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is TreeViewItem { Header: ValueNode propertyNode })
            {
                viewModelEngine
                    .OnNext(propertyNode)
                     .Subscribe(node =>
                     {
                         if (node is ValueNode valueNode)
                         {
                             ViewModelTree.Items.Clear();
                             controller.TreeView(valueNode, ViewModelTree)
                                 .Subscribe(treeView =>
                                 {
                                 });
                         }
                         else
                             throw new Exception("F33dsssd22");
                     });
                //ViewModelTree.SelectedObject = property;
            }
            else
                throw new Exception("dfgf 543432eee");
        }
    }
}
