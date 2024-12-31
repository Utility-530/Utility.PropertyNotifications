using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace Utility.WPF.Controls.Breadcrumbs
{
    public class BreadCrumbTree : TreeView
    {
        static BreadCrumbTree()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BreadCrumbTree), new FrameworkPropertyMetadata(typeof(BreadCrumbTree)));
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new BreachCrumb();
        }
    }
}
