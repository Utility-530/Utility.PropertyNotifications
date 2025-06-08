using System.Windows.Controls;
using System.Windows;
using Utility.WPF.Attached;

namespace Utility.WPF.Controls.Trees
{
    public class CustomTreeView : TreeView
    {
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new CustomTreeViewItem() { ItemContainerStyleSelector = ItemContainerStyleSelector, ItemContainerStyle = ItemContainerStyle };
        }


        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {

            TreeEx.SetIsRootNode(element, true);
            if (element is CustomTreeViewItem treeviewitem)
            {
                treeviewitem.Checked += (s, e) => RaiseEvent(new CheckedEventArgs(e.IsChecked, CheckedEvent, s));
                treeviewitem.HierarchyMouseUp += (s, e) => RaiseEvent(new HierarchyMouseUpEventArgs(HierarchyMouseUpEvent, this));
            }
            else            
            {

            }
            base.PrepareContainerForItemOverride(element, item);

        }

        public static readonly RoutedEvent CheckedEvent = EventManager.RegisterRoutedEvent(
    name: "Checked",
    routingStrategy: RoutingStrategy.Bubble,
    handlerType: typeof(CheckedRoutedEventHandler),
    ownerType: typeof(CustomTreeView));


        public static readonly RoutedEvent HierarchyMouseUpEvent = EventManager.RegisterRoutedEvent(
    name: "HierarchyMouseUp",
    routingStrategy: RoutingStrategy.Bubble,
    handlerType: typeof(HierarchyMouseUpEventHandler),
    ownerType: typeof(CustomTreeView));



        public event CheckedRoutedEventHandler Checked
        {
            add { AddHandler(CheckedEvent, value); }
            remove { RemoveHandler(CheckedEvent, value); }
        }


        public event HierarchyMouseUpEventHandler HierarchyMouseUp
        {
            add { AddHandler(HierarchyMouseUpEvent, value); }
            remove { RemoveHandler(HierarchyMouseUpEvent, value); }
        }
    }
}
