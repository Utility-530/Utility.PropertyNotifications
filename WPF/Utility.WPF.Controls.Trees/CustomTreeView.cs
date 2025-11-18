using System.Windows;
using System.Windows.Controls;
using Utility.WPF.Attached;

namespace Utility.WPF.Controls.Trees
{
    public class CustomTreeView : TreeView
    {
        public static readonly DependencyProperty SelectedItemTemplateSelectorProperty =
    DependencyProperty.Register(nameof(SelectedItemTemplateSelector), typeof(DataTemplateSelector), typeof(CustomTreeView), new PropertyMetadata());



        public StyleSelector SelectedItemStyleSelector
        {
            get { return (StyleSelector)GetValue(SelectedItemStyleSelectorProperty); }
            set { SetValue(SelectedItemStyleSelectorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedItemStyleSelector.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedItemStyleSelectorProperty =
            DependencyProperty.Register(nameof(SelectedItemStyleSelector), typeof(StyleSelector), typeof(CustomTreeView), new PropertyMetadata());




        protected override DependencyObject GetContainerForItemOverride()
        {
            return new CustomTreeViewItem()
            {
                ItemContainerStyleSelector = ItemContainerStyleSelector,
                ItemContainerStyle = ItemContainerStyle,
                SelectedItemTemplateSelector = SelectedItemTemplateSelector,
                ItemTemplateSelector = ItemTemplateSelector,
                TreeView = this
            };
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



        public DataTemplateSelector SelectedItemTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(SelectedItemTemplateSelectorProperty); }
            set { SetValue(SelectedItemTemplateSelectorProperty, value); }
        }
    }
}