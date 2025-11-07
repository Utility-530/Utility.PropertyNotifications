using System.Collections;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Utility.EventArguments;
using Utility.WPF.Controls.Trees;

namespace Utility.WPF.Controls.ComboBoxes
{
    public class ComboBoxTreeView : ComboBox
    {
        public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.Register("SelectedItems", typeof(IEnumerable), typeof(ComboBoxTreeView), new PropertyMetadata(null));
        public static readonly DependencyProperty ParentPathProperty = DependencyProperty.Register("ParentPath", typeof(string), typeof(ComboBoxTreeView), new PropertyMetadata());
        public static readonly DependencyProperty SelectedNodeProperty = DependencyProperty.Register("SelectedNode", typeof(object), typeof(ComboBoxTreeView), new FrameworkPropertyMetadata(default, CoerceValueCallback));
        public static readonly DependencyProperty SelectedItemTemplateProperty = DependencyProperty.Register("SelectedItemTemplate", typeof(DataTemplate), typeof(ComboBoxTreeView), new PropertyMetadata());
        public static readonly DependencyProperty SelectedItemTemplateSelectorProperty = DependencyProperty.Register("SelectedItemTemplateSelector", typeof(DataTemplateSelector), typeof(ComboBoxTreeView), new PropertyMetadata());
        public static readonly DependencyProperty IsErrorProperty = DependencyProperty.Register("IsError", typeof(bool), typeof(ComboBoxTreeView), new PropertyMetadata(false));
        public static readonly RoutedEvent SelectedNodeChangedEvent = EventManager.RegisterRoutedEvent("SelectedNodeChanged", RoutingStrategy.Bubble, typeof(SelectedNodeEventHandler), typeof(ComboBoxTreeView));
        public static readonly RoutedEvent CheckedItemsChangedEvent = EventManager.RegisterRoutedEvent("CheckedItemsChanged", RoutingStrategy.Bubble, typeof(CheckedItemsChangedEventHandler), typeof(ComboBoxTreeView));
        public static readonly DependencyProperty ToggleButtonContentProperty = DependencyProperty.Register("ToggleButtonContent", typeof(object), typeof(ComboBoxTreeView), new PropertyMetadata());
        public static readonly DependencyProperty TreeItemContainerStyleProperty = DependencyProperty.Register("TreeItemContainerStyle", typeof(Style), typeof(ComboBoxTreeView), new PropertyMetadata());
        public static readonly DependencyProperty ToggleButtonTemplateProperty = DependencyProperty.Register("ToggleButtonTemplate", typeof(ControlTemplate), typeof(ComboBoxTreeView), new PropertyMetadata());
        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register("IsOpen", typeof(bool), typeof(ComboBoxTreeView), new PropertyMetadata(_changed));

        public event EventHandler<ValueCoercingEventArgs>? ValueCoercing;

        private static object CoerceValueCallback(DependencyObject d, object baseValue)
        {
            object newValue = baseValue;
            var control = (ComboBoxTreeView)d;
            var args = new ValueCoercingEventArgs(newValue, control.SelectedNode);

            if (control.ValueCoercing == null)
                return baseValue;
            // Fire event for subscribers to approve/modify
            control.ValueCoercing?.Invoke(d, args);

            // If subscriber rejected the value, revert to current value
            if (args.Cancel)
                return control.SelectedNode;

            return baseValue;
        }

        private static void _changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ComboBoxTreeView treeView && e.NewValue is bool b)
            {
                if (!treeView.IsFocused)
                {
                    treeView.Focus();
                }
                //treeView.ToggleButton.IsChecked = b;
                treeView.IsDropDownOpen = b;
                treeView.Popup.IsOpen = b;
            }
        }

        public delegate void SelectedNodeEventHandler(object sender, SelectedNodeEventArgs e);

        public delegate void CheckedItemsChangedEventHandler(object sender, CheckedItemsEventArgs e);

        public class SelectedNodeEventArgs : RoutedEventArgs
        {
            public SelectedNodeEventArgs(RoutedEvent routedEvent, object source, object value) : base(routedEvent, source)
            {
                Value = value;
            }

            public object Value { get; }
        }

        public class CheckedItemsEventArgs : RoutedEventArgs
        {
            public CheckedItemsEventArgs(RoutedEvent routedEvent, object source, IEnumerable checkedItems, IEnumerable unCheckedItems) : base(routedEvent, source)
            {
                CheckedItems = checkedItems;
                UnCheckedItems = unCheckedItems;
            }

            public IEnumerable CheckedItems { get; }
            public IEnumerable UnCheckedItems { get; }
        }

        public ComboBoxTreeView()
        {
            this.DropDownOpened += ComboBoxTreeView_DropDownOpened;
            this.LostFocus += ComboBoxTreeView_LostFocus;
            SelectedItems = List;
        }

        private void ComboBoxTreeView_DropDownOpened(object? sender, EventArgs e)
        {
        }

        private void ComboBoxTreeView_LostFocus(object sender, RoutedEventArgs e)
        {
            IsOpen = false;
            //IsDropDownOpen = false;
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            IsOpen = false;
            IsDropDownOpen = false;
            base.OnMouseLeave(e);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            IsOpen = false;
            IsDropDownOpen = false;
            base.OnMouseLeftButtonDown(e);
        }

        private void ComboBoxTreeView_DropDownClosed(object? sender, EventArgs e)
        {
            IsOpen = false;
        }

        public CustomTreeView TreeView { get; set; }
        public ToggleButton ToggleButton { get; set; }
        public Popup Popup { get; set; }

        public ObservableCollection<object> List = new();

        static ComboBoxTreeView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ComboBoxTreeView), new FrameworkPropertyMetadata(typeof(ComboBoxTreeView)));
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            //don't call the method of the base class
        }

        public override void OnApplyTemplate()
        {
            this.DropDownClosed += ComboBoxTreeView_DropDownClosed;
            Popup = (Popup)GetTemplateChild("Popup");
            ToggleButton = (ToggleButton)GetTemplateChild("ToggleButton");
            ToggleButton.Checked += ToggleButton_Checked;
            TreeView = (CustomTreeView)GetTemplateChild("treeView");
            TreeView.SelectedItemChanged += TreeView_SelectedItemChanged;
            TreeView.HierarchyMouseUp += (s, e) => OnTreeViewHierarchyMouseUp(s, e);
            TreeView.Checked += _treeView_OnChecked;
            if (TreeView.SelectedItem != null)
                UpdateSelectedItems(TreeView.SelectedItem);
            base.OnApplyTemplate();
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (TreeView.SelectedItem != null)
            {
                var hierarchy = SelectItems(TreeView.SelectedItem);

                //SelectedItems = hierarchy;
                UpdateSelectedItems(TreeView.SelectedItem);

                //IsDropDownOpen = false;
            }
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            if (ToggleButton.IsChecked == false)
            {
            }
        }

        private void _treeView_OnChecked(object sender, RoutedEventArgs e)
        {
            if (e is CheckedEventArgs { IsChecked: bool isChecked, OriginalSource: CustomTreeViewItem item })
            {
                CheckedItemsEventArgs args = null;
                if (isChecked)
                {
                    List.Add(item.DataContext);
                    args = new CheckedItemsEventArgs(CheckedItemsChangedEvent, this, new object[] { item.DataContext }, null);
                }
                else if (List.Contains(item.DataContext))
                {
                    List.Remove(item.DataContext);
                    args = new CheckedItemsEventArgs(CheckedItemsChangedEvent, this, null, new object[] { item.DataContext });
                }
                IsDropDownOpen = false;

                RaiseEvent(args);
            }
        }

        protected override void OnDropDownClosed(EventArgs e)
        {
            if (TreeView?.SelectedItem != null)
                UpdateSelectedItems(TreeView.SelectedItem);
        }

        protected override void OnDropDownOpened(EventArgs e)
        {
            base.OnDropDownOpened(e);
            if (TreeView?.SelectedItem != null)
                UpdateSelectedItems(TreeView.SelectedItem);
        }

        /// <summary>
        /// Handles clicks on any item in the tree view
        /// </summary>
        private void OnTreeViewHierarchyMouseUp(object sender, RoutedEventArgs e)
        {
            if (TreeView.SelectedItem != null)
            {
                //IsDropDownOpen = false;
                ToggleButton.IsChecked = false;
            }
        }

        private void RaiseSelectedNodeChangedEvent(object value, IEnumerable selectedItems)
        {
            RoutedEventArgs newEventArgs = new SelectedNodeEventArgs(SelectedNodeChangedEvent, this, value);
            RaiseEvent(newEventArgs);
        }

        #region properties

        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        public DataTemplate SelectedItemTemplate
        {
            get { return (DataTemplate)GetValue(SelectedItemTemplateProperty); }
            set { SetValue(SelectedItemTemplateProperty, value); }
        }

        public ControlTemplate ToggleButtonTemplate
        {
            get { return (ControlTemplate)GetValue(ToggleButtonTemplateProperty); }
            set { SetValue(ToggleButtonTemplateProperty, value); }
        }

        public Style TreeItemContainerStyle
        {
            get { return (Style)GetValue(TreeItemContainerStyleProperty); }
            set { SetValue(TreeItemContainerStyleProperty, value); }
        }

        public string ParentPath
        {
            get { return (string)GetValue(ParentPathProperty); }
            set { SetValue(ParentPathProperty, value); }
        }

        public object SelectedNode
        {
            get { return GetValue(SelectedNodeProperty); }
            set { SetValue(SelectedNodeProperty, value); }
        }

        public IEnumerable SelectedItems
        {
            get { return (IEnumerable)GetValue(SelectedItemsProperty); }
            set { SetValue(SelectedItemsProperty, value); }
        }

        public object ToggleButtonContent
        {
            get { return (object)GetValue(ToggleButtonContentProperty); }
            set { SetValue(ToggleButtonContentProperty, value); }
        }

        public DataTemplateSelector SelectedItemTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(SelectedItemTemplateSelectorProperty); }
            set { SetValue(SelectedItemTemplateSelectorProperty, value); }
        }

        public bool IsError
        {
            get { return (bool)GetValue(IsErrorProperty); }
            set { SetValue(IsErrorProperty, value); }
        }

        public event SelectedNodeEventHandler SelectedNodeChanged
        {
            add { AddHandler(SelectedNodeChangedEvent, value); }
            remove { RemoveHandler(SelectedNodeChangedEvent, value); }
        }

        public event CheckedItemsChangedEventHandler CheckedItemsChanged
        {
            add { AddHandler(CheckedItemsChangedEvent, value); }
            remove { RemoveHandler(CheckedItemsChangedEvent, value); }
        }

        #endregion properties

        public void UpdateSelectedItems(object selectedItem)
        {
            var hierarchy = SelectItems(selectedItem);
            SelectedItems = hierarchy;
            SelectedNode = selectedItem;
            RaiseSelectedNodeChangedEvent(selectedItem, SelectedItems);
        }

        private object[] SelectItems(object selectedItem)
        {
            var type = selectedItem.GetType();
            var propInfo = type.GetProperty(ParentPath);
            return propInfo == null
                ? throw new Exception($"{type.Name} && {ParentPath}")
                :
                [.. TreeHelper.GetAncestors(selectedItem, a => {
                if (!propInfo.DeclaringType.IsInstanceOfType(a))
                {
                    throw new InvalidOperationException(
                        $"Object of type {a.GetType().Name} does not derive from {propInfo.DeclaringType.Name}"
                    );
                }
                return propInfo.GetValue(a);
                }).Reverse()];
        }
    }

    internal static class TreeHelper
    {
        public static IEnumerable<object> GetAncestors(object vm, Func<object, object> parent)
        {
            yield return vm;
            while (parent(vm) != null)
            {
                yield return parent(vm);
                vm = parent(vm);
            }
        }
    }
}