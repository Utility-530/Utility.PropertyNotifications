using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Utility.WPF.Controls.Trees
{
    public class ComboBoxTreeView : ComboBox
    {
        public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.Register("SelectedItems", typeof(IEnumerable), typeof(ComboBoxTreeView), new PropertyMetadata(null));
        public static readonly DependencyProperty ParentPathProperty = DependencyProperty.Register("ParentPath", typeof(string), typeof(ComboBoxTreeView), new PropertyMetadata());
        public static readonly DependencyProperty SelectedNodeProperty = DependencyProperty.Register("SelectedNode", typeof(object), typeof(ComboBoxTreeView), new FrameworkPropertyMetadata(default));
        public static readonly DependencyProperty IsCheckedPathProperty = DependencyProperty.Register("IsCheckedPath", typeof(string), typeof(ComboBoxTreeView), new PropertyMetadata());
        public static readonly DependencyProperty IsExpandedPathProperty = DependencyProperty.Register("IsExpandedPath", typeof(string), typeof(ComboBoxTreeView), new PropertyMetadata("IsExpanded"));
        public static readonly DependencyProperty IsSelectedPathProperty = DependencyProperty.Register("IsSelectedPath", typeof(string), typeof(ComboBoxTreeView), new PropertyMetadata("IsSelected"));
        public static readonly DependencyProperty SelectedItemTemplateProperty = DependencyProperty.Register("SelectedItemTemplate", typeof(DataTemplate), typeof(ComboBoxTreeView), new PropertyMetadata());


        public bool IsError
        {
            get { return (bool)GetValue(IsErrorProperty); }
            set { SetValue(IsErrorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsError.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsErrorProperty =
            DependencyProperty.Register("IsError", typeof(bool), typeof(ComboBoxTreeView), new PropertyMetadata(false));


        protected ExtendedTreeView _treeView;
        private ObservableCollection<object> list = new();

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
            _treeView = (ExtendedTreeView)GetTemplateChild("treeView");
            _treeView.OnHierarchyMouseUp += new MouseEventHandler(OnTreeViewHierarchyMouseUp);
            _treeView.OnChecked += _treeView_OnChecked;
            if (_treeView.SelectedItem != null)
                UpdateSelectedItem(_treeView.SelectedItem);
            base.OnApplyTemplate();
        }

        private void _treeView_OnChecked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckableTreeViewItem { IsChecked: bool isChecked } item)
            {
                if (isChecked)
                {
                    list.Add(item.DataContext);
                }
                else if (list.Contains(item))
                {
                    list.Remove(item.DataContext);
                }
                SelectedItems = list;
            }
        }

        protected override void OnDropDownClosed(EventArgs e)
        {
            base.OnDropDownClosed(e);
            if (_treeView.SelectedItem != null)
                UpdateSelectedItem(_treeView.SelectedItem);
        }

        protected override void OnDropDownOpened(EventArgs e)
        {
            base.OnDropDownOpened(e);
            if (_treeView.SelectedItem != null)
                UpdateSelectedItem(_treeView.SelectedItem);
        }

        /// <summary>
        /// Handles clicks on any item in the tree view
        /// </summary>
        private void OnTreeViewHierarchyMouseUp(object sender, MouseEventArgs e)
        {
            if (_treeView.SelectedItem != null)
            {
                var hierarchy = SelectItems(_treeView.SelectedItem);
                SelectedItem = hierarchy.First();
                SelectedItems = hierarchy;
                UpdateSelectedItem(_treeView.SelectedItem);
                IsDropDownOpen = false;
                SelectedNode = _treeView.SelectedItem;
            }
        }

        #region properties

        public DataTemplate SelectedItemTemplate
        {
            get { return (DataTemplate)GetValue(SelectedItemTemplateProperty); }
            set { SetValue(SelectedItemTemplateProperty, value); }
        }

        public string IsExpandedPath
        {
            get { return (string)GetValue(IsExpandedPathProperty); }
            set { SetValue(IsExpandedPathProperty, value); }
        }

        public string IsSelectedPath
        {
            get { return (string)GetValue(IsSelectedPathProperty); }
            set { SetValue(IsSelectedPathProperty, value); }
        }

        public string IsCheckedPath
        {
            get { return (string)GetValue(IsCheckedPathProperty); }
            set { SetValue(IsCheckedPathProperty, value); }
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

        #endregion properties

        protected void UpdateSelectedItem(object selectedItem)
        {

            var hierarchy = SelectItems(selectedItem);
            SelectedItems = hierarchy;
            SelectedNode = hierarchy.Last();

        }

        private object[] SelectItems(object selectedItem)
        {
            var type = selectedItem.GetType();
            var propInfo = type.GetProperty(ParentPath);
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
