using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Utility.Interfaces.NonGeneric;
using Utility.PropertyDescriptors;
using Utility.Trees.Abstractions;

namespace Utility.Meta.WPF.Demo.Descriptors
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var treeView = new CustomTreeView() { };
            treeView.ItemTemplate = CustomTreeView.create(); 
            treeView.ItemsSource = new[] { DescriptorFactory.CreateRoot(new List<Model> { new Model() }) };
            var window = new Window { Content = treeView };
            window.Show();
            base.OnStartup(e);
        }
    }

    public class CustomTreeView : TreeView
    {
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new CustomTreeViewItem() { IsExpanded = true, ItemTemplate = ItemTemplate };
        }

        public static DataTemplate create()
        {
            var itemTemplate = Utility.WPF.Factorys.TemplateGenerator.CreateHierarcialDataTemplate(() =>
            {
                var contentControl = new ContentControl() { };
                contentControl.SetBinding(ContentControl.ContentProperty, new Binding() { Path = new PropertyPath(".") });
                return contentControl;
            }, nameof(IYieldChildren.Children));
            return itemTemplate;
        }
    }

    public class CustomTreeViewItem : TreeViewItem
    {
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new CustomTreeViewItem() { IsExpanded = true, ItemTemplate = ItemTemplate };
        }
    }


    public class Model
    {
        public string Name { get; set; }
    }
}
