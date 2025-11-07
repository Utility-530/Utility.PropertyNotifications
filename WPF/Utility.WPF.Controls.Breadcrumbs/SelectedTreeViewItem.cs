using System.Windows;
using System.Windows.Controls;

namespace Utility.WPF.Controls.Breadcrumbs
{
    public class SelectedTreeViewItem : TreeViewItem
    {
        static SelectedTreeViewItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SelectedTreeViewItem), new FrameworkPropertyMetadata(typeof(SelectedTreeViewItem)));
        }
    }
}