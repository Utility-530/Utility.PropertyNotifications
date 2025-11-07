using System.Windows;
using System.Windows.Controls;

namespace Utility.WPF.Controls.Breadcrumbs
{
    public class BreachCrumb : TreeViewItem
    {
        static BreachCrumb()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BreachCrumb), new FrameworkPropertyMetadata(typeof(BreachCrumb)));
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new SelectedTreeViewItem();
        }
    }
}