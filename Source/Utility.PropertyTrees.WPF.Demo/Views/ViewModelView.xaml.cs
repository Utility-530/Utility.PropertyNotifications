using DryIoc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static Utility.PropertyTrees.WPF.Demo.LightBootStrapper;
using static Utility.PropertyTrees.WPF.Demo.Views.PropertyView;

namespace Utility.PropertyTrees.WPF.Demo.Views
{
    /// <summary>
    /// Interaction logic for ViewModelView.xaml
    /// </summary>
    public partial class ViewModelView : UserControl
    {
        private ViewModelEngine viewModelEngine => container.Resolve<ViewModelEngine>();
        private MainViewModel viewModel => container.Resolve<MainViewModel>();
        private PropertyNode masterNode => container.Resolve<PropertyNode>(Keys.Model);

        public ViewModelView()
        {
            InitializeComponent();
            initialise_click(default, default);
        }

        private void initialise_click(object sender, RoutedEventArgs e)
        {
            //this.PropertyTree.SelectedObject = viewModel.Model;
            viewModel.TreeView(masterNode, PropertyTree).Subscribe(treeView =>
            {
                //PropertyTree.Children.Clear();
                //PropertyTree.Children.Add(treeView);
            });
        }

        private void PropertyTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is TreeViewItem { Header: PropertyNode propertyNode })
            {
                viewModelEngine
                    .Observe(propertyNode)
                    .Cast<PropertyNode>()
                     .Subscribe(node =>
                     {
                         ViewModelTree.Items.Clear();
                         viewModel.TreeView(node, ViewModelTree)
                             .Subscribe(treeView =>
                             {
                             });
                     });
                //ViewModelTree.SelectedObject = property;
            }
            else
                throw new Exception("dfgf 543432eee");
        }
    }
}
