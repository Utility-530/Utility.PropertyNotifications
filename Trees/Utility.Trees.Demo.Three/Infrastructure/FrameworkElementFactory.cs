using System.Windows;
using System.Windows.Controls.Primitives;
using Utility.Trees.Demo.MVVM.Infrastructure;
using Views.Trees;

namespace Utility.Trees.Demo.MVVM
{
    public partial class App
    {

        public Window CreateWindow()
        {
            var grid = new UniformGrid() { Rows = 1 };
            grid.Children.Add(ModelTreeViewer(model));
            grid.Children.Add(ViewModelTreeViewer(viewModel));
            grid.Children.Add(ViewTreeViewer(view));
            var window = new Window { Content = grid };
            return window;
        }


        public static TreeViewer ModelTreeViewer(object model)
        {
            return new TreeViewer
            {
                ViewModel = model,
                TreeViewItemFactory = Default.TreeViewItemFactory.Instance,
                TreeViewBuilder = TreeViewBuilder.Instance,
                PanelsConverter = Default.ItemsPanelConverter.Instance,
                DataTemplateSelector = Default.DataTemplateSelector.Instance,
                TreeViewFilter = Model.Filter.Instance,
                StyleSelector = Model.StyleSelector.Instance,
                EventListener = Default.EventListener.Instance
            };
        }

        public static TreeViewer ViewModelTreeViewer(object viewModel)
        {
            return new TreeViewer
            {
                ViewModel = viewModel,
                TreeViewItemFactory = Default.TreeViewItemFactory.Instance,
                TreeViewBuilder = TreeViewBuilder.Instance,
                PanelsConverter = Default.ItemsPanelConverter.Instance,
                DataTemplateSelector = Default.DataTemplateSelector.Instance,
                TreeViewFilter = Default.Filter.Instance,
                StyleSelector = ViewModel.StyleSelector.Instance,
                EventListener = Default.EventListener.Instance
            };
        }

        public static TreeViewer ViewTreeViewer(object view)
        {
            return new TreeViewer
            {
                ViewModel = view,
                TreeViewItemFactory = Default.TreeViewItemFactory.Instance,
                TreeViewBuilder = TreeViewBuilder.Instance,

                PanelsConverter = View.ItemsPanelConverter.Instance,
                DataTemplateSelector = View.DataTemplateSelector.Instance,
                StyleSelector = View.StyleSelector.Instance,

                TreeViewFilter = Default.Filter.Instance,
                EventListener = Default.EventListener.Instance
            };
        }

    }

}
