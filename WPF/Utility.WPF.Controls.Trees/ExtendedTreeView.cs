using System.Windows;
using System.Windows.Controls;

namespace Utility.WPF.Controls.Trees
{
    public partial class CustomTreeViewItem
    {
        public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.Register("IsChecked", typeof(bool), typeof(CustomTreeViewItem), new PropertyMetadata(false));

        public void _OnApplyTemplate()
        {
            MouseLeftButtonUp += (s, e) => OnMouseLeftButtonUp(s);
            if (GetTemplateChild("PART_CheckBox") is CheckBox checkBox)
            {
                checkBox.Checked += CheckBox_Checked;
                checkBox.Unchecked += CheckBox_Unchecked;
            }
        }

        private void OnMouseLeftButtonUp(object sender)
        {
            RaiseEvent(new HierarchyMouseUpEventArgs(HierarchyMouseUpEvent, sender));
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new CheckedEventArgs(false, CheckedEvent, sender));
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new CheckedEventArgs(true, CheckedEvent, sender));
        }

        private void ChildItem_IsCheckedHandler(object sender, CheckedEventArgs e)
        {
            RaiseEvent(e);
            e.Handled = true;
        }

        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        public static readonly RoutedEvent CheckedEvent = EventManager.RegisterRoutedEvent(
            name: "Checked",
            routingStrategy: RoutingStrategy.Bubble,
            handlerType: typeof(CheckedRoutedEventHandler),
            ownerType: typeof(CustomTreeViewItem));

        public static readonly RoutedEvent HierarchyMouseUpEvent = EventManager.RegisterRoutedEvent(
            name: "HierarchyMouseUp",
            routingStrategy: RoutingStrategy.Bubble,
            handlerType: typeof(HierarchyMouseUpEventHandler),
            ownerType: typeof(CustomTreeViewItem));

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