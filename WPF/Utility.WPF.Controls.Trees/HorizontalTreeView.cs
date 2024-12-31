using System.Windows;
using System.Windows.Controls;

namespace Utility.WPF.Controls.Trees
{
    public class HorizontalTreeView : TreeView
    {
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new HorizontalTreeViewItem();
        }

    }


    public class HorizontalTreeViewItem : TreeViewItem
    {
        static HorizontalTreeViewItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HorizontalTreeViewItem), new FrameworkPropertyMetadata(typeof(HorizontalTreeViewItem)));
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new HorizontalTreeViewItem();

        }
    }
}
