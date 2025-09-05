using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Utility.WPF.Controls.Trees;

namespace Utility.WPF.Controls.ComboBoxes
{


    public class ComboBoxTreeView : ComboBox
    {
        public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.Register("SelectedItems", typeof(IEnumerable), typeof(ComboBoxTreeView), new PropertyMetadata(null));
        public static readonly DependencyProperty ParentPathProperty = DependencyProperty.Register("ParentPath", typeof(string), typeof(ComboBoxTreeView), new PropertyMetadata());
        public static readonly DependencyProperty SelectedNodeProperty = DependencyProperty.Register("SelectedNode", typeof(object), typeof(ComboBoxTreeView), new FrameworkPropertyMetadata(default));
        //public static readonly DependencyProperty IsCheckedPathProperty = DependencyProperty.Register("IsCheckedPath", typeof(string), typeof(ComboBoxTreeView), new PropertyMetadata());
        //public static readonly DependencyProperty IsExpandedPathProperty = DependencyProperty.Register("IsExpandedPath", typeof(string), typeof(ComboBoxTreeView), new PropertyMetadata("IsExpanded"));
        //public static readonly DependencyProperty IsSelectedPathProperty = DependencyProperty.Register("IsSelectedPath", typeof(string), typeof(ComboBoxTreeView), new PropertyMetadata("IsSelected"));
        public static readonly DependencyProperty SelectedItemTemplateProperty = DependencyProperty.Register("SelectedItemTemplate", typeof(DataTemplate), typeof(ComboBoxTreeView), new PropertyMetadata());
        public static readonly DependencyProperty SelectedItemTemplateSelectorProperty = DependencyProperty.Register("SelectedItemTemplateSelector", typeof(DataTemplateSelector), typeof(ComboBoxTreeView), new PropertyMetadata());
        public static readonly DependencyProperty IsErrorProperty = DependencyProperty.Register("IsError", typeof(bool), typeof(ComboBoxTreeView), new PropertyMetadata(false));
        public static readonly RoutedEvent SelectedNodeChangedEvent = EventManager.RegisterRoutedEvent("SelectedNodeChanged", RoutingStrategy.Bubble, typeof(SelectedNodeEventHandler), typeof(ComboBoxTreeView));
        public static readonly DependencyProperty ToggleButtonContentProperty = DependencyProperty.Register("ToggleButtonContent", typeof(object), typeof(ComboBoxTreeView), new PropertyMetadata());
        public static readonly DependencyProperty TreeItemContainerStyleProperty = DependencyProperty.Register("TreeItemContainerStyle", typeof(Style), typeof(ComboBoxTreeView), new PropertyMetadata());
        public static readonly DependencyProperty ToggleButtonTemplateProperty =    DependencyProperty.Register("ToggleButtonTemplate", typeof(ControlTemplate), typeof(ComboBoxTreeView), new PropertyMetadata());




        public delegate void SelectedNodeEventHandler(object sender, SelectedNodeEventArgs e);

        public class SelectedNodeEventArgs : RoutedEventArgs
        {
            public SelectedNodeEventArgs(RoutedEvent routedEvent, object source, object value) : base(routedEvent, source)
            {
                Value = value;
            }

            public object Value { get; }
        }
        public ComboBoxTreeView()
        {
            this.DropDownClosed += ComboBoxTreeView_DropDownClosed;
            this.LostFocus += ComboBoxTreeView_LostFocus;

        }

        private void ComboBoxTreeView_LostFocus(object sender, RoutedEventArgs e)
        {
            //IsDropDownOpen = false;
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            IsDropDownOpen = false;
            base.OnMouseLeave(e);
        }


        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            IsDropDownOpen = false;
            base.OnMouseLeftButtonDown(e);
        }
        private void ComboBoxTreeView_DropDownClosed(object? sender, EventArgs e)
        {
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

            //Popup.MouseEnter += (s, e) => e.Handled = true;
            //Popup.MouseLeave += (s, e) => e.Handled = true;


            TreeView = (CustomTreeView)GetTemplateChild("treeView");
            TreeView.SelectedItemChanged += TreeView_SelectedItemChanged;
            TreeView.HierarchyMouseUp += (s, e) => OnTreeViewHierarchyMouseUp(s, e);
            TreeView.Checked += _treeView_OnChecked;
            if (TreeView.SelectedItem != null)
                UpdateSelectedItems(TreeView.SelectedItem);
            base.OnApplyTemplate();
        }

        private void ComboBoxTreeView_DropDownClosed1(object? sender, EventArgs e)
        {

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
            if (sender is CustomTreeViewItem { IsChecked: bool isChecked } item)
            {
                if (isChecked)
                {
                    List.Add(item.DataContext);
                }
                else if (List.Contains(item))
                {
                    List.Remove(item.DataContext);
                }
                SelectedItems = List;
            }
            IsDropDownOpen = false;
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

        private void RaiseSelectedNodeChangedEvent(object value)
        {
            RoutedEventArgs newEventArgs = new SelectedNodeEventArgs(SelectedNodeChangedEvent, this, value);
            RaiseEvent(newEventArgs);
        }

        #region properties

        public DataTemplate SelectedItemTemplate
        {
            get { return (DataTemplate)GetValue(SelectedItemTemplateProperty); }
            set { SetValue(SelectedItemTemplateProperty, value); }
        }

        //public string IsExpandedPath
        //{
        //    get { return (string)GetValue(IsExpandedPathProperty); }
        //    set { SetValue(IsExpandedPathProperty, value); }
        //}

        //public string IsSelectedPath
        //{
        //    get { return (string)GetValue(IsSelectedPathProperty); }
        //    set { SetValue(IsSelectedPathProperty, value); }
        //}

        //public string IsCheckedPath
        //{
        //    get { return (string)GetValue(IsCheckedPathProperty); }
        //    set { SetValue(IsCheckedPathProperty, value); }
        //}
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

        #endregion properties

        public void UpdateSelectedItems(object selectedItem)
        {

            var hierarchy = SelectItems(selectedItem);
            SelectedItems = hierarchy;
            SelectedNode = selectedItem;
            RaiseSelectedNodeChangedEvent(selectedItem);
        }

        private object[] SelectItems(object selectedItem)
        {
            var type = selectedItem.GetType();
            var propInfo = type.GetProperty(ParentPath);
            if (propInfo == null)
            {
                throw new Exception($"{type.Name} && {ParentPath}");
            }
            return TreeHelper.GetAncestors(selectedItem, a => propInfo.GetValue(a)).Reverse().ToArray();
        }
    }

    static class TreeHelper
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
