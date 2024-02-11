using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Utility.WPF.Controls.Trees
{
    public class ExtendedTreeView : TreeView
    {

        public event MouseEventHandler? OnHierarchyMouseUp;
        public event RoutedEventHandler? OnChecked;

        public static readonly DependencyProperty IsExpandedPathProperty = DependencyProperty.Register("IsExpandedPath", typeof(string), typeof(ExtendedTreeView), new PropertyMetadata());
        public static readonly DependencyProperty IsSelectedPathProperty = DependencyProperty.Register("IsSelectedPath", typeof(string), typeof(ExtendedTreeView), new PropertyMetadata());
        public static readonly DependencyProperty IsCheckedPathProperty = DependencyProperty.Register("IsCheckedPath", typeof(string), typeof(ExtendedTreeView), new PropertyMetadata());

        public string IsSelectedPath
        {
            get { return (string)GetValue(IsSelectedPathProperty); }
            set { SetValue(IsSelectedPathProperty, value); }
        }

        public string IsExpandedPath
        {
            get { return (string)GetValue(IsExpandedPathProperty); }
            set { SetValue(IsExpandedPathProperty, value); }
        }

        public string IsCheckedPath
        {
            get { return (string)GetValue(IsCheckedPathProperty); }
            set { SetValue(IsCheckedPathProperty, value); }
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            ExtendedTreeViewItem treeViewItem = null;
            if (IsCheckedPath == null)
            {
                treeViewItem = ExtendedTreeViewItem.CreateItemWithBinding(IsExpandedPath, IsSelectedPath);
            }
            else
            {
                var xtreeViewItem = CheckableTreeViewItem.CreateItemWithBinding(IsExpandedPath, IsSelectedPath, IsCheckedPath);
                xtreeViewItem.IsCheckedHandler += XtreeViewItem_IsCheckedHandler;
                treeViewItem = xtreeViewItem;
            }

            treeViewItem.OnHierarchyMouseUp += OnChildItemMouseLeftButtonUp;


            return treeViewItem;
        }

        private void XtreeViewItem_IsCheckedHandler(object sender, RoutedEventArgs e)
        {
            if (OnChecked != null)
            {
                OnChecked(sender, e);
                e.Handled = true;
            }
        }

        private void OnChildItemMouseLeftButtonUp(object sender, MouseEventArgs e)
        {
            if (OnHierarchyMouseUp != null)
            {
                OnHierarchyMouseUp(this, e);
                e.Handled = true;
            }
        }

    }

    public class ExtendedTreeViewItem : TreeViewItem
    {
        private string isExpandedPath;
        private string isSelectedPath;

        public ExtendedTreeViewItem()
        {
            MouseLeftButtonUp += OnMouseLeftButtonUp;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        public ExtendedTreeViewItem(string isExpandedPath, string isSelectedPath)
        {
            this.isExpandedPath = isExpandedPath;
            this.isSelectedPath = isSelectedPath;
        }

        void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            HierarchyMouseUp(e);
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            var childItem = CreateItemWithBinding(isExpandedPath, isSelectedPath);

            childItem.MouseLeftButtonUp += OnMouseLeftButtonUp;

            return childItem;
        }

        public static ExtendedTreeViewItem CreateItemWithBinding(string isExpandedPath, string isSelectedPath)
        {
            var tvi = new ExtendedTreeViewItem(isExpandedPath, isSelectedPath);

            var expandedBinding = new Binding(isExpandedPath)
            {
                Mode = BindingMode.TwoWay
            };
            tvi.SetBinding(IsExpandedProperty, expandedBinding);

            var selectedBinding = new Binding(isSelectedPath)
            {
                Mode = BindingMode.TwoWay
            };
            tvi.SetBinding(IsSelectedProperty, selectedBinding);

            return tvi;
        }

        public event MouseEventHandler OnHierarchyMouseUp;

        protected void HierarchyMouseUp(MouseButtonEventArgs e)
        {
            if (OnHierarchyMouseUp != null)
            {
                OnHierarchyMouseUp?.Invoke(this, e);
                e.Handled = true;
            }
        }
    }



    public class CheckableTreeViewItem : ExtendedTreeViewItem
    {
        private string isExpandedPath;
        private string isSelectedPath;
        private string isCheckedPath;
        private CheckBox? checkBox;

        public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.Register("IsChecked", typeof(bool), typeof(CheckableTreeViewItem), new PropertyMetadata(false));

        static CheckableTreeViewItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CheckableTreeViewItem), new FrameworkPropertyMetadata(typeof(CheckableTreeViewItem)));
        }

        public CheckableTreeViewItem()
        {
        }

        public override void OnApplyTemplate()
        {
            checkBox = GetTemplateChild("PART_CheckBox") as CheckBox;
            checkBox.Checked += CheckBox_Checked;
            checkBox.Unchecked += CheckBox_Unchecked;
            base.OnApplyTemplate();
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (IsCheckedHandler != null)
            {
                IsCheckedHandler?.Invoke(this, e);
                e.Handled = true;
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (IsCheckedHandler != null)
            {
                IsCheckedHandler?.Invoke(this, e);
                e.Handled = true;
            }
        }

        public CheckableTreeViewItem(string isExpandedPath, string isSelectedPath, string isCheckedPath)
        {
            this.isExpandedPath = isExpandedPath;
            this.isSelectedPath = isSelectedPath;
            this.isCheckedPath = isCheckedPath;
        }

        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }


        protected override DependencyObject GetContainerForItemOverride()
        {
            var childItem = CreateItemWithBinding(isExpandedPath, isSelectedPath, isCheckedPath);
            childItem.IsCheckedHandler += ChildItem_IsCheckedHandler;
            return childItem;
        }

        private void ChildItem_IsCheckedHandler(object sender, RoutedEventArgs e)
        {
            if (IsCheckedHandler != null)
            {
                IsCheckedHandler?.Invoke(sender, e);
                e.Handled = true;
            }
        }

        public static CheckableTreeViewItem CreateItemWithBinding(string isExpandedPath, string isSelectedPath, string isCheckedPath)
        {
            var tvi = new CheckableTreeViewItem(isExpandedPath, isSelectedPath, isCheckedPath);

            var expandedBinding = new Binding(isExpandedPath)
            {
                Mode = BindingMode.TwoWay
            };
            tvi.SetBinding(IsExpandedProperty, expandedBinding);

            var selectedBinding = new Binding(isSelectedPath)
            {
                Mode = BindingMode.TwoWay
            };
            tvi.SetBinding(IsSelectedProperty, selectedBinding);

            var isCheckedBinding = new Binding(isCheckedPath)
            {
                Mode = BindingMode.TwoWay
            };
            tvi.SetBinding(IsCheckedProperty, isCheckedBinding);

            return tvi;
        }

        public event RoutedEventHandler IsCheckedHandler;
    }
}
